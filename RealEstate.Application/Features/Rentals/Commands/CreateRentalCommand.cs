using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Dtos.Rental;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Features.Rentals.Commands
{
    public class CreateRentalCommand : IRequest<AppResponse<Guid>>
    {
        public CreateRentalDTO Data { get; }

        public CreateRentalCommand(CreateRentalDTO date)
        {
            Data = date;
        }

    }



    public class CreateRentalCommandHandler : IRequestHandler<CreateRentalCommand, AppResponse<Guid>>
    {
        protected readonly IPropertyRepository _propertyRepository;
        protected readonly ICustomerRepository _customerRepository; 
        private readonly IFileManager _fileManager;
        private readonly IRentalsRepository _RentalsRepository;
        private readonly IMapper _mapper;
        private Guid _lessorId = Guid.Empty;
        public CreateRentalCommandHandler(
            IPropertyRepository propertyRepository,
            ICustomerRepository customerRepository, 
            IFileManager fileManager,
            IRentalsRepository RentalsRepository,
            IMapper mapper)
        {
            this._propertyRepository = propertyRepository;
            this._customerRepository = customerRepository; 
            this._fileManager = fileManager;
            this._RentalsRepository = RentalsRepository;
            this._mapper = mapper;
        }

        public async Task<AppResponse<Guid>> Handle(CreateRentalCommand request, CancellationToken cancellationToken)
        {

            var validationResults = await _ValideteRentalData(request);

            if (validationResults.IsFailed)
            {
                return AppResponse<Guid>.Fail(validationResults.Errors);
            }

            var PCIU = await _PerformContractImageUrl(request.Data.ContractImageUrl);

            if (PCIU.IsFailed)
            {
                return AppResponse<Guid>.Fail(PCIU.Errors);
            }
            var Rental = new Rental
            {
                PropertyId = Guid.Parse(request.Data.PropertyId!),
                LesseeId = Guid.Parse(request.Data.LesseeId!),
                LessorId = this._lessorId,
                RentPriceMonth = request.Data.RentPrice,
                StartDate = DateOnly.FromDateTime(request.Data.StartDate ?? DateTime.UtcNow),
                Duration = (short)request.Data.Duration,
                RentType = request.Data.RentType,
                Description = request.Data.Description ?? "",
                ContractImageUrl = PCIU.Value
            };


            var dbContext = _RentalsRepository.GetDbContext();
            var property = await _propertyRepository.GetByIdAsync(Guid.Parse(request.Data.PropertyId!));
            using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                await _RentalsRepository.AddAsync(Rental); 
                property.PropertyStatus = PropertyStatus.Rented;
                await _RentalsRepository.SaveChangesAsync();


                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AppResponse<Guid>.Fail(new InternalServerError("Creation" , ex.Message, enApiErrorCode.CreationFailed));
            }


            return AppResponse<Guid>.Success(Rental.Id);
        }

       

        private async Task<Result> _ValideteRentalData(CreateRentalCommand request)
        {
            List<Error> errors = new();
            var isPropertyIdGuid = Guid.TryParse(request.Data.PropertyId, out var propertyId); 
            var isLesseeIdGuid = Guid.TryParse(request.Data.LesseeId, out var lesseeId);
       

            if (!isPropertyIdGuid ||  !isLesseeIdGuid    )
            {
                if (!isPropertyIdGuid)
                    errors.Add(new ValidationError("propertyId", "Invalid GUID format", enApiErrorCode.InvalidGuid));
                
                else if (!isLesseeIdGuid)
                    errors.Add(new ValidationError("LesseeId", "Invalid GUID format", enApiErrorCode.InvalidGuid));
             
            }


       

            if (!_propertyRepository.IsPropertyExistsById(Guid.Parse(request.Data.PropertyId!)))
            {
                errors.Add(new ValidationError("PropertyId", $"Not Found Property With Id {request.Data.PropertyId}", enApiErrorCode.PropertyNotFound));
            }

            var property = await _propertyRepository.GetByIdAsync(Guid.Parse(request.Data.PropertyId!));
            if (property == null)
            {
                errors.Add(new ConflictError("PropertyId", "This property is not available for Rental.", enApiErrorCode.NotAvailable));
            } else
            {
                this._lessorId = property.OwnerId;
            }


            if (!_customerRepository.IsCustomerExists(Guid.Parse(request.Data.LesseeId!)))
            {
                errors.Add(new ValidationError("LesseeId", $"Not Found Seller With Id {request.Data.LesseeId}", enApiErrorCode.CustomerNotFound));
            }
            return errors.Any() ? Result.Fail(errors) : Result.Ok();
        }

        private  async Task<Result<string>> _PerformContractImageUrl(IFormFile image)
        {
            return await  _fileManager.SaveRentalContractImageAsync(image);
        }
    }
}
