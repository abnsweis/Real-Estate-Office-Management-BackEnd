using AutoMapper;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Dtos.Property;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using System.Linq.Expressions;

namespace RealEstate.Application.Features.Properties.Querys.Get
{
    /// <summary>
    /// Query for retrieving a single property by its unique identifier
    /// </summary>
    public class GetPropertyByIdQuery : IRequest<AppResponse<PropertyDTO>>
    {
        /// <summary>
        /// The unique identifier of the property to retrieve
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Initializes a new instance of the GetPropertyByIdQuery class
        /// </summary>
        /// <param name="Id">The unique identifier of the property</param>
        public GetPropertyByIdQuery(Guid Id)
        {
            this.Id = Id;
        }
    }

    /// <summary>
    /// Handler for the GetPropertyByIdQuery that retrieves a property from the repository
    /// </summary>
    public class GetPropertyByIdQueryHndler : IRequestHandler<GetPropertyByIdQuery, AppResponse<PropertyDTO>>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IFileManager _fileManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the GetPropertyByIdQueryHndler class
        /// </summary>
        /// <param name="propertyRepository">Property repository interface</param>
        /// <param name="customerRepository">Customer repository interface (unused in current implementation)</param>
        /// <param name="fileManager">File manager service for handling media files</param>
        /// <param name="mapper">AutoMapper instance for object mapping</param>
        public GetPropertyByIdQueryHndler(
            IPropertyRepository propertyRepository,
            ICustomerRepository customerRepository,
            IFileManager fileManager,
            IMapper mapper)
        {
            _propertyRepository = propertyRepository;
            _fileManager = fileManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the property retrieval request by ID
        /// </summary>
        /// <param name="request">The query request containing the property ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Successful response containing the property DTO if found, 
        /// or failure response with NotFoundError if property doesn't exist or is deleted
        /// </returns>
        public async Task<AppResponse<PropertyDTO>> Handle(
            GetPropertyByIdQuery request,
            CancellationToken cancellationToken)
        {
            // Create filter expression to find non-deleted property by ID
            Expression<Func<Property, bool>> filter = p =>
                p.Id == request.Id && !p.IsDeleted;

            // Retrieve property from repository with related entities included
            var property = await _propertyRepository.FirstOrDefaultAsync(
                filter,
                includes: new Expression<Func<Property, object>>[] {
                    p => p.Category,
                    p => p.Owner.Person,
                    p => p.PropertyImages,  
                    p => p.Ratings
                }
            );

            // Return not found response if property doesn't exist
            if (property is null)
            {
                return AppResponse<PropertyDTO>.Fail(new NotFoundError(
                    "Property",
                    "PropertyId",
                    request.Id.ToString(),
                    enApiErrorCode.PropertyNotFound
                ));
            }

            // Map the entity to DTO
            var propertyDto = _mapper.Map<PropertyDTO>(property);

            // Convert stored file paths to public URLs for images
            for (int i = 0; i < propertyDto.Images.Count; i++)
            {
                propertyDto.Images[i] = _fileManager.GetPublicURL(propertyDto.Images[i]);
            }

            // Convert stored file path to public URL for video if exists
            if (propertyDto.VideoUrl is not null)
            {
                propertyDto.VideoUrl = _fileManager.GetPublicURL(propertyDto.VideoUrl);
            }
            if (propertyDto.MainImage is not null)
            {
                var img = _fileManager.GetPublicURL(propertyDto.MainImage);
                propertyDto.MainImage = img;
            }
            // Return successful response with the property data
            var response = AppResponse<PropertyDTO>.Success(propertyDto);
            return response;
        }
    }
}