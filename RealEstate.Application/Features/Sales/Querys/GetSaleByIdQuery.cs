using AutoMapper;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Dtos.Sales;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using System.Linq.Expressions;

namespace RealEstate.Application.Features.Sales.Querys
{
    /// <summary>
    /// Query for retrieving a single sale by its unique identifier
    /// </summary>
    public class GetSaleByIdQuery : IRequest<AppResponse<SaleDTO>>
    {
        /// <summary>
        /// The unique identifier of the Sale to retrieve
        /// </summary>
        public Guid SaleId { get; }

        /// <summary>
        /// Initializes a new instance of the GetSaleByIdQuery class
        /// </summary>
        /// <param name="Id">The unique identifier of the Sale</param>
        public GetSaleByIdQuery(Guid saleId)
        {
            this.SaleId = saleId;
        }
    }

    /// <summary>
    /// Handler for the GetSaleByIdQuery that retrieves a Sale from the repository
    /// </summary>
    public class GetSaleByIdQueryHndler : IRequestHandler<GetSaleByIdQuery, AppResponse<SaleDTO>>
    {
        private readonly ISalesRepository _SaleRepository;
        private readonly IFileManager _fileManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the GetSaleByIdQueryHndler class
        /// </summary>
        /// <param name="SaleRepository">Sale repository interface</param>
        /// <param name="customerRepository">Customer repository interface (unused in current implementation)</param>
        /// <param name="fileManager">File manager service for handling media files</param>
        /// <param name="mapper">AutoMapper instance for object mapping</param>
        public GetSaleByIdQueryHndler(
            ISalesRepository SaleRepository,
            ICustomerRepository customerRepository,
            IFileManager fileManager,
            IMapper mapper)
        {
            _SaleRepository = SaleRepository;
            _fileManager = fileManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the Sale retrieval request by ID
        /// </summary>
        /// <param name="request">The query request containing the Sale ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Successful response containing the Sale DTO if found, 
        /// or failure response with NotFoundError if Sale doesn't exist or is deleted
        /// </returns>
        public async Task<AppResponse<SaleDTO>> Handle(
            GetSaleByIdQuery request,
            CancellationToken cancellationToken)
        {
            // Create filter expression to find non-deleted Sale by ID
            Expression<Func<Sale, bool>> filter = p =>
                p.Id == request.SaleId && !p.IsDeleted;

            // Retrieve Sale from repository with related entities included
            var Sale = await _SaleRepository.FirstOrDefaultAsync(
                filter,
                includes: new Expression<Func<Sale, object>>[] {
                    p => p.Property,
                    p => p.Property.Category,
                    p => p.Seller,
                    p => p.Seller.Person,
                    p => p.Buyer.Person,
                    p => p.Buyer
                }
            );

            // Return not found response if Sale doesn't exist
            if (Sale is null)
            {
                return AppResponse<SaleDTO>.Fail(new NotFoundError(
                    "Sale",
                    "SaleId",
                    request.SaleId.ToString(),
                    enApiErrorCode.SaleNotFound
                ));
            }

            // Map the entity to DTO
            var SaleDto = _mapper.Map<SaleDTO>(Sale);

            if (SaleDto.ContractImageUrl is not null)
            {
                SaleDto.ContractImageUrl = _fileManager.GetPublicURL(SaleDto.ContractImageUrl);
            }

            // Return successful response with the Sale data
            var response = AppResponse<SaleDTO>.Success(SaleDto);
            return response;
        }
    }
}
