using AutoMapper;
using MediatR;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Dtos.Rental;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Entities;
using System.Linq.Expressions;

namespace RealEstate.Application.Features.Rentals.Querys
{

    public class GetRentalsByLesseeIdQuery : IRequest<AppResponse<PaginationResponse<RentalDTO>>>
    {

        public PaginationRequest Pagination { get; set; }
        public Guid LesseeId { get; }

        public GetRentalsByLesseeIdQuery(PaginationRequest pagination, Guid buyerId)
        {
            Pagination = pagination;
            LesseeId = buyerId;
        }
    }


    public class GetRentalsByLesseeIdQueryHandler : IRequestHandler<GetRentalsByLesseeIdQuery, AppResponse<PaginationResponse<RentalDTO>>>
    {

        private readonly IRentalsRepository _rentalsRepository;
        private readonly IFileManager _fileManager;
        private readonly IMapper _mapper;

        public GetRentalsByLesseeIdQueryHandler(
            IRentalsRepository RentalsRepository,
            IFileManager fileManager,
            IMapper mapper)
        {
            _rentalsRepository = RentalsRepository;
            _fileManager = fileManager;
            _mapper = mapper;
        }

        public async Task<AppResponse<PaginationResponse<RentalDTO>>> Handle(GetRentalsByLesseeIdQuery request, CancellationToken cancellationToken)
        {


            // Retrieve Rentals from repository with pagination, and includes
            var Rentals = await _rentalsRepository.GetAllAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                filter: s => s.LesseeId == request.LesseeId,
                orderBy: q => q.OrderBy(p => p.Property.Title), // Order by title
                includes: new Expression<Func<Rental, object>>[] {
                    p => p.Property,
                    p => p.Property.Category,
                    p => p.Lessee,
                    p => p.Lessee.Person,
                    p => p.Lessor.Person,
                    p => p.Lessor
                }
            );

            // Get total count of properties (after filtering)
            var totalCount = await _rentalsRepository.CountAsync();

            // Create paginated response
            var paginatedResponse = new PaginationResponse<RentalDTO>
            {
                Items = _mapper.Map<List<RentalDTO>>(Rentals),
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize,
                TotalCount = totalCount
            };
            // Process media URLs for each Rental
            foreach (var item in paginatedResponse.Items)
            {
                // Convert video path to public URL if exists
                if (item.ContractImageUrl is not null)
                {
                    item.ContractImageUrl = _fileManager.GetPublicURL(item.ContractImageUrl);
                }
            }
            // Return successful response with the paginated data
            var response = AppResponse<PaginationResponse<RentalDTO>>.Success(paginatedResponse);
            return response;
        }
    }
}