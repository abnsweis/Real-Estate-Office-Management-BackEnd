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
    /// Query for retrieving a property by its unique property number
    /// </summary>
    public class GetPropertyByNumberQuery : IRequest<AppResponse<PropertyDTO>>
    {
        /// <summary>
        /// The unique property number used to identify the property
        /// </summary>
        public string PropertyNumber { get; }

        /// <summary>
        /// Initializes a new instance of the GetPropertyByNumberQuery class
        /// </summary>
        /// <param name="propertyNumber">The property's unique identification number</param>
        public GetPropertyByNumberQuery(string propertyNumber)
        {
            PropertyNumber = propertyNumber;
        }
    }

    /// <summary>
    /// Handler for retrieving property details by property number
    /// </summary>
    public class GetPropertyByNumberQueryHndler : IRequestHandler<GetPropertyByNumberQuery, AppResponse<PropertyDTO>>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IFileManager _fileManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the GetPropertyByNumberQueryHndler class
        /// </summary>
        /// <param name="propertyRepository">Repository for property data access</param>
        /// <param name="customerRepository">Customer repository (currently unused)</param>
        /// <param name="fileManager">Service for managing file storage and URLs</param>
        /// <param name="mapper">Object mapper for DTO conversions</param>
        public GetPropertyByNumberQueryHndler(
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
        /// Handles the request to retrieve a property by its number
        /// </summary>
        /// <param name="request">The query containing the property number</param>
        /// <param name="cancellationToken">Cancellation token for async operations</param>
        /// <returns>
        /// Successful response with property details if found,
        /// or error response if property doesn't exist or is deleted
        /// </returns>
        public async Task<AppResponse<PropertyDTO>> Handle(
            GetPropertyByNumberQuery request,
            CancellationToken cancellationToken)
        {
            // Create filter to find non-deleted property by exact property number match
            Expression<Func<Property, bool>> filter = p =>
                p.PropertyNumber.ToString() == request.PropertyNumber &&
                !p.IsDeleted;

            // Retrieve property with related entities (category, owner/person, and images)
            var property = await _propertyRepository.FirstOrDefaultAsync(
                filter,
                includes: new Expression<Func<Property, object>>[] {
                    p => p.Category,
                    p => p.Owner.Person,
                    p => p.PropertyImages,
                    p => p.Ratings
                }
            );

            // Return not found error if property doesn't exist
            if (property is null)
            {
                return AppResponse<PropertyDTO>.Fail(new NotFoundError(
                    entituName: "Property",
                    propertyName: "PropertyNumber",
                    entityErrorValue: request.PropertyNumber,
                    errorCode: enApiErrorCode.PropertyNotFound
                ));
            }

            // Map entity to DTO
            var propertyDto = _mapper.Map<PropertyDTO>(property);

            // Convert stored file paths to public URLs for all images
            for (int i = 0; i < propertyDto.Images.Count; i++)
            {
                propertyDto.Images[i] = _fileManager.GetPublicURL(propertyDto.Images[i]);
            }

            // Convert video path to public URL if exists
            if (propertyDto.VideoUrl is not null)
            {
                propertyDto.VideoUrl = _fileManager.GetPublicURL(propertyDto.VideoUrl);
            }
            if (propertyDto.MainImage is not null)
            {
                var img = _fileManager.GetPublicURL(propertyDto.MainImage);
                propertyDto.MainImage = img;
            }
            // Return successful response with property data
            return AppResponse<PropertyDTO>.Success(propertyDto);
        }
    }
}