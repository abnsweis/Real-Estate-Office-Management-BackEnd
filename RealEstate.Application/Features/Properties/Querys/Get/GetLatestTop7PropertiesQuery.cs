using AutoMapper;
using MediatR;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Dtos.Property;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Entities;
using System.Linq.Expressions;

namespace RealEstate.Application.Features.Properties.Querys
{
    public class GetLatestTop7PropertiesQuery : IRequest<AppResponse<List<PropertyDTO>>>{}
    public class GetLatestTop7PropertiesQueryHandler : IRequestHandler<GetLatestTop7PropertiesQuery, AppResponse<List<PropertyDTO>>>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IFileManager _fileManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the GetLatestTop7PropertiesQueryHandler class
        /// </summary>
        /// <param name="propertyRepository">Property repository interface</param>
        /// <param name="fileManager">File manager service for handling media files</param>
        /// <param name="mapper">AutoMapper instance for object mapping</param>
        public GetLatestTop7PropertiesQueryHandler(
            IPropertyRepository propertyRepository,
            IFileManager fileManager,
            IMapper mapper)
        {
            _propertyRepository = propertyRepository;
            _fileManager = fileManager;
            _mapper = mapper;
        }


        public async Task<AppResponse<List<PropertyDTO>>> Handle(GetLatestTop7PropertiesQuery request, CancellationToken cancellationToken)
        {

            var icludes = new Expression<Func<Property, object>>[] {
                    p => p.Category,
                    p => p.Owner.Person,
                    p => p.PropertyImages,
                    p => p.Ratings
                };
            var properties = await _propertyRepository.GetLatestTop7Properties(icludes);
            var propertiesDTO = _mapper.Map<List<PropertyDTO>>(properties);
            // Process media URLs for each property
            foreach (var item in propertiesDTO)
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

                if (item.MainImage is not null)
                {
                    var img = _fileManager.GetPublicURL(item.MainImage);
                    item.MainImage = img;
                }
            }
            // Return successful response with the paginated data
            var response = AppResponse<List<PropertyDTO>>.Success(propertiesDTO);
            return response;
        }
    }
}
