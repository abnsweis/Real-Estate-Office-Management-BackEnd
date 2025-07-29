using AutoMapper;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Dtos.Rental;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using System.Linq.Expressions;

namespace RealEstate.Application.Features.Rentals.Querys
{
    /// <summary>
    /// Query for retrieving a single Rental by its unique identifier
    /// </summary>
    public class GetRentalByIdQuery : IRequest<AppResponse<RentalDTO>>
    {
        /// <summary>
        /// The unique identifier of the Rental to retrieve
        /// </summary>
        public Guid RentalId { get; }

        /// <summary>
        /// Initializes a new instance of the GetRentalByIdQuery class
        /// </summary>
        /// <param name="Id">The unique identifier of the Rental</param>
        public GetRentalByIdQuery(Guid RentalId)
        {
            this.RentalId = RentalId;
        }
    }

    /// <summary>
    /// Handler for the GetRentalByIdQuery that retrieves a Rental from the repository
    /// </summary>
    public class GetRentalByIdQueryHndler : IRequestHandler<GetRentalByIdQuery, AppResponse<RentalDTO>>
    {
        private readonly IRentalsRepository _RentalRepository; 
        private readonly IFileManager _fileManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the GetRentalByIdQueryHndler class
        /// </summary>
        /// <param name="RentalRepository">Rental repository interface</param>
        /// <param name="customerRepository">Customer repository interface (unused in current implementation)</param>
        /// <param name="fileManager">File manager service for handling media files</param>
        /// <param name="mapper">AutoMapper instance for object mapping</param>
        public GetRentalByIdQueryHndler(
            IRentalsRepository RentalRepository,
            ICustomerRepository customerRepository,
            IFileManager fileManager,
            IMapper mapper)
        {
            _RentalRepository = RentalRepository;
            _fileManager = fileManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the Rental retrieval request by ID
        /// </summary>
        /// <param name="request">The query request containing the Rental ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Successful response containing the Rental DTO if found, 
        /// or failure response with NotFoundError if Rental doesn't exist or is deleted
        /// </returns>
        public async Task<AppResponse<RentalDTO>> Handle(
            GetRentalByIdQuery request,
            CancellationToken cancellationToken)
        {
            // Create filter expression to find non-deleted Rental by ID
            Expression<Func<Rental, bool>> filter = p =>
                p.Id == request.RentalId && !p.IsDeleted;

            // Retrieve Rental from repository with related entities included
            var Rental = await _RentalRepository.FirstOrDefaultAsync(
                filter,
                includes: new Expression<Func<Rental, object>>[] {
                    p => p.Property,
                    p => p.Property.Category,
                    p => p.Lessee,
                    p => p.Lessee.Person,
                    p => p.Lessor.Person,
                    p => p.Lessor
                }
            );

            // Return not found response if Rental doesn't exist
            if (Rental is null)
            {
                return AppResponse<RentalDTO>.Fail(new NotFoundError(
                    "Rental",
                    "RentalId",
                    request.RentalId.ToString(),
                    enApiErrorCode.RentalNotFound
                ));
            }

            // Map the entity to DTO
            var RentalDto = _mapper.Map<RentalDTO>(Rental);

            if (RentalDto.ContractImageUrl is not null)
            {
                RentalDto.ContractImageUrl = _fileManager.GetPublicURL(RentalDto.ContractImageUrl);
            }

            // Return successful response with the Rental data
            var response = AppResponse<RentalDTO>.Success(RentalDto);
            return response;
        }
    }
}
