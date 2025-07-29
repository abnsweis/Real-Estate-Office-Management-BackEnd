using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Dtos.Sales;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Features.Sales.Commands
{
    public class CreateSaleCommand : IRequest<AppResponse<Guid>>
    {
        public CreateSaleDTO Data { get; }

        public CreateSaleCommand(CreateSaleDTO date)
        {
            Data = date;
        }

    }



    public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, AppResponse<Guid>>
    {
        protected readonly IPropertyRepository _propertyRepository;
        protected readonly ICustomerRepository _customerRepository;
        protected readonly ICategoryRepository _categoryRepository;
        private readonly IFileManager _fileManager;
        private readonly ISalesRepository _salesRepository;
        private readonly IMapper _mapper;

        public CreateSaleCommandHandler(
            IPropertyRepository propertyRepository,
            ICustomerRepository customerRepository,
            ICategoryRepository categoryRepository,
            IFileManager fileManager,
            ISalesRepository salesRepository,
            IMapper mapper)
        {
            this._propertyRepository = propertyRepository;
            this._customerRepository = customerRepository;
            this._categoryRepository = categoryRepository;
            this._fileManager = fileManager;
            this._salesRepository = salesRepository;
            this._mapper = mapper;
        }

        public async Task<AppResponse<Guid>> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {

            var validationResults = _ValideteSaleData(request);

            if (validationResults.IsFailed)
            {
                return AppResponse<Guid>.Fail(validationResults.Errors);
            }

            var PCIU = await _PerformContractImageUrl(request.Data.ContractImage);

            if (PCIU.IsFailed)
            {
                return AppResponse<Guid>.Fail(PCIU.Errors);
            }
            var sale = new Sale
            {
                PropertyId = Guid.Parse(request.Data.PropertyId!),
                SellerId = Guid.Parse(request.Data.SellerId!),
                BuyerId = Guid.Parse(request.Data.BuyerId!),
                Price = request.Data.Price,
                SaleDate = DateOnly.Parse(request.Data.SaleDate!),
                Description = request.Data.Description ?? "",
                ContractImageUrl = PCIU.Value
            };


            var dbContext = _salesRepository.GetDbContext();
            var property = await _propertyRepository.GetByIdAsync(Guid.Parse(request.Data.PropertyId!));
            using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                await _salesRepository.AddAsync(sale);

                property!.OwnerId = Guid.Parse(request.Data.BuyerId!);
                property.PropertyStatus = enPropertyStatus.Sold;
                await _salesRepository.SaveChangesAsync();


                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AppResponse<Guid>.Fail(new InternalServerError("Creation", ex.Message, enApiErrorCode.CreationFailed));
            }


            return AppResponse<Guid>.Success(sale.Id);
        }



        private Result _ValideteSaleData(CreateSaleCommand request)
        {
            List<Error> errors = new();
            var isPropertyIdGuid = Guid.TryParse(request.Data.PropertyId, out var propertyId);
            var isSellerIdGuid = Guid.TryParse(request.Data.SellerId, out var sellerId);
            var isBuyerIdGuid = Guid.TryParse(request.Data.BuyerId, out var buyerId);
            var IsValidSaleDate = DateOnly.TryParse(request.Data.SaleDate, out var saleDate);



            if (!isPropertyIdGuid || !isSellerIdGuid || !isBuyerIdGuid || !IsValidSaleDate)
            {
                if (!isPropertyIdGuid)
                    errors.Add(new ValidationError("propertyId", "Invalid GUID format", enApiErrorCode.InvalidGuid));
                else if (!isSellerIdGuid)
                    errors.Add(new ValidationError("sellerId", "Invalid GUID format", enApiErrorCode.InvalidGuid));
                else if (!isBuyerIdGuid)
                    errors.Add(new ValidationError("buyerId", "Invalid GUID format", enApiErrorCode.InvalidGuid));
                else if (!IsValidSaleDate)
                    errors.Add(new ValidationError("SaleDate", "Invalid SaleDate format", enApiErrorCode.InvalidDate));

            }


            if (!_customerRepository.IsCustomerExists(Guid.Parse(request.Data.SellerId!)))
            {
                errors.Add(new ValidationError("SellerId", $"Not Found Seller With Id {request.Data.SellerId}", enApiErrorCode.CustomerNotFound));
            }
            if (!_customerRepository.IsCustomerExists(Guid.Parse(request.Data.BuyerId!)))
            {
                errors.Add(new ValidationError("BuyerId", $"Not Found Buyer With Id {request.Data.BuyerId}", enApiErrorCode.CustomerNotFound));
            }

            if (!_propertyRepository.IsPropertyExistsById(Guid.Parse(request.Data.PropertyId!)))
            {
                errors.Add(new ValidationError("PropertyId", $"Not Found Property With Id {request.Data.PropertyId}", enApiErrorCode.PropertyNotFound));
            }


            if (request.Data.SellerId == request.Data.BuyerId)
            {
                errors.Add(new ConflictError("SellerId", "Seller and Buyer cannot be the same person.", enApiErrorCode.SellerAndBuyerCannotBeSame));
            }



            if (_propertyRepository.GetPropertyOwnerId(propertyId) != sellerId)
            {
                errors.Add(new ConflictError("SellerId", "Seller is not the actual owner of the property.", enApiErrorCode.SellerNotOwner));
            }

            if (_propertyRepository.IsPropertyAvailable(propertyId))
            {
                errors.Add(new ConflictError("PropertyId", "This property is not available for sale.", enApiErrorCode.NotAvailable));
            }

            return errors.Any() ? Result.Fail(errors) : Result.Ok();
        }

        private async Task<Result<string>> _PerformContractImageUrl(IFormFile image)
        {
            return await _fileManager.SaveSaleContractImageAsync(image);
        }
    }
}
