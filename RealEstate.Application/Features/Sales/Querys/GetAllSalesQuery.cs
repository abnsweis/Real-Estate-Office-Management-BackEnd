using AutoMapper;
using MediatR;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Dtos.Property;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Dtos.Sales;
using RealEstate.Domain.Entities;
using System.Linq.Expressions;

namespace RealEstate.Application.Features.Sales.Querys
{

    public class GetAllSalesQuery : IRequest<AppResponse<PaginationResponse<SaleDTO>>>
    {
        /// <summary>
        /// Pagination parameters (page number and page size)
        /// </summary>
        public PaginationRequest Pagination { get; set; }


        /// <summary>
        /// Initializes a new instance of the GetAllSalesQuery class
        /// </summary>
        /// <param name="pagination">Pagination parameters</param>
        /// <param name="filter">Filter criteria</param>
        public GetAllSalesQuery(PaginationRequest pagination)
        {
            Pagination = pagination;
        }
    }

    /// <summary>
    /// Handler for the GetAllSalesQuery that retrieves sales from the repository
    /// </summary>
    public class GetAllSalesQueryHandler : IRequestHandler<GetAllSalesQuery, AppResponse<PaginationResponse<SaleDTO>>>
    {

        private readonly ISalesRepository _salesRepository;
        private readonly IFileManager _fileManager;
        private readonly IMapper _mapper;
 
        public GetAllSalesQueryHandler(
            ISalesRepository salesRepository,
            IFileManager fileManager,
            IMapper mapper)
        {
            _salesRepository = salesRepository;
            _fileManager = fileManager;
            _mapper = mapper;
        }

        public async Task<AppResponse<PaginationResponse<SaleDTO>>> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
        {


            // Retrieve Sales from repository with pagination, and includes
            var sales = await _salesRepository.GetAllAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                null,
                orderBy: q => q.OrderBy(p => p.Property.Title), // Order by title
                includes: new Expression<Func<Sale, object>>[] {
                    p => p.Property,
                    p => p.Property.Category,
                    p => p.Seller,
                    p => p.Seller.Person,
                    p => p.Buyer.Person,
                    p => p.Buyer
                }
            );

            // Get total count of properties (after filtering)
            var totalCount = await _salesRepository.CountAsync();

            // Create paginated response
            var paginatedResponse = new PaginationResponse<SaleDTO>
            {
                Items = _mapper.Map<List<SaleDTO>>(sales),
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize,
                TotalCount = totalCount
            };
            // Process media URLs for each sale
            foreach (var item in paginatedResponse.Items)
            {  
                // Convert video path to public URL if exists
                if (item.ContractImageUrl is not null)
                {
                    item.ContractImageUrl = _fileManager.GetPublicURL(item.ContractImageUrl);
                }
            }
            // Return successful response with the paginated data
            var response = AppResponse<PaginationResponse<SaleDTO>>.Success(paginatedResponse);
            return response;
        }
    }
}