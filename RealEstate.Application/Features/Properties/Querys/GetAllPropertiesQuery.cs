using AutoMapper;
using MediatR;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Dtos.Category;
using RealEstate.Application.Dtos.Property;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Features.Categories.Querys;
using RealEstate.Domain.Entities;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RealEstate.Application.Features.Properties.Querys
{
    /// <summary>
    /// Query for retrieving all properties with optional filtering and pagination
    /// </summary>
    public class GetAllPropertiesQuery : IRequest<AppResponse<PaginationResponse<PropertyDTO>>>
    {
        /// <summary>
        /// Pagination parameters (page number and page size)
        /// </summary>
        public PaginationRequest Pagination { get; set; }

        /// <summary>
        /// Filter criteria for properties
        /// </summary>
        public FilterPropertiesDTO Filter { get; set; }

        /// <summary>
        /// Initializes a new instance of the GetAllPropertiesQuery class
        /// </summary>
        /// <param name="pagination">Pagination parameters</param>
        /// <param name="filter">Filter criteria</param>
        public GetAllPropertiesQuery(PaginationRequest pagination, FilterPropertiesDTO filter)
        {
            Pagination = pagination;
            Filter = filter;
        }
    }

    /// <summary>
    /// Handler for the GetAllPropertiesQuery that retrieves properties from the repository
    /// </summary>
    public class GetAllPropertiesQueryHandler : IRequestHandler<GetAllPropertiesQuery, AppResponse<PaginationResponse<PropertyDTO>>>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IFileManager _fileManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the GetAllPropertiesQueryHandler class
        /// </summary>
        /// <param name="propertyRepository">Property repository interface</param>
        /// <param name="fileManager">File manager service for handling media files</param>
        /// <param name="mapper">AutoMapper instance for object mapping</param>
        public GetAllPropertiesQueryHandler(
            IPropertyRepository propertyRepository,
            IFileManager fileManager,
            IMapper mapper)
        {
            _propertyRepository = propertyRepository;
            _fileManager = fileManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the property retrieval request with filtering and pagination
        /// </summary>
        /// <param name="request">The query request containing pagination and filter parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated response containing property DTOs</returns>
        public async Task<AppResponse<PaginationResponse<PropertyDTO>>> Handle(
            GetAllPropertiesQuery request,
            CancellationToken cancellationToken)
        {
            // Build the filter expression based on request parameters
            Expression<Func<Property, bool>> filter = property =>
                (string.IsNullOrEmpty(request.Filter.Title) || property.Title.StartsWith(request.Filter.Title)) &&
                (string.IsNullOrEmpty(request.Filter.Location) || property.Location.StartsWith(request.Filter.Location)) &&
                (!request.Filter.MinPrice.HasValue || property.Price >= request.Filter.MinPrice.Value) &&
                (!request.Filter.MaxPrice.HasValue || property.Price <= request.Filter.MaxPrice.Value) &&
                property.IsDeleted == false;

            // Retrieve properties from repository with pagination, filtering, and includes
            var properties = await _propertyRepository.GetAllAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                filter,
                orderBy: q => q.OrderBy(p => p.Title), // Order by title
                includes: new Expression<Func<Property, object>>[] {
                    p => p.Category,
                    p => p.Owner.Person,
                    p => p.PropertyImages,
                    p => p.Ratings
                }
            );

            // Get total count of properties (after filtering)
            var totalCount = await _propertyRepository.CountAsync();

            // Create paginated response
            var paginatedResponse = new PaginationResponse<PropertyDTO>
            {
                Items = _mapper.Map<List<PropertyDTO>>(properties),
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize,
                TotalCount = totalCount
            };



            // Process media URLs for each property
            foreach (var item in paginatedResponse.Items)
            {
                // Convert image paths to public URLs
                for (int i = 0; i < item.Images.Count; i++)
                {
                    item.Images[i] = _fileManager.GetPublicURL(item.Images[i]);
                };

                // Convert video path to public URL if exists
                if (item.VideoUrl is not null)
                {
                    item.VideoUrl = _fileManager.GetPublicURL(item.VideoUrl);
                }
               if(item.MainImage is not null)
                {
                    var img = _fileManager.GetPublicURL(item.MainImage);
                    item.MainImage = img;
                }

            }




            // Return successful response with the paginated data
            var response = AppResponse<PaginationResponse<PropertyDTO>>.Success(paginatedResponse);
            return response;
        }
    }
}