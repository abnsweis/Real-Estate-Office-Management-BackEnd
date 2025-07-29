using AutoMapper;
using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Dtos.Property;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using System.Linq.Expressions;

namespace RealEstate.Application.Features.Properties.Querys.Filter
{

    public class GetPropertiesByOwnerIdQuery : IRequest<AppResponse<PaginationResponse<PropertyDTO>>>
    {
        public Guid OwnerId { get; }

        public PaginationRequest Pagination { get; set; }

        public GetPropertiesByOwnerIdQuery(PaginationRequest pagination, Guid ownerId)
        {
            Pagination = pagination;
            OwnerId = ownerId;
        }
    }


    public class GetPropertiesByOwnerIdQueryHndler : IRequestHandler<GetPropertiesByOwnerIdQuery, AppResponse<PaginationResponse<PropertyDTO>>>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IFileManager _fileManager;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public GetPropertiesByOwnerIdQueryHndler(
            IPropertyRepository propertyRepository,
            ICustomerRepository customerRepository,
                        IFileManager fileManager,
            IMapper mapper)
        {
            _propertyRepository = propertyRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
            this._fileManager = fileManager;
        }

        public async Task<AppResponse<PaginationResponse<PropertyDTO>>> Handle(GetPropertiesByOwnerIdQuery request, CancellationToken cancellationToken)
        {

            if (!_customerRepository.IsCustomerExists(request.OwnerId))
            {
                return AppResponse<PaginationResponse<PropertyDTO>>.Fail(new NotFoundError(
                    "Owner",
                    "OwnerId",
                    request.OwnerId.ToString(),
                    enApiErrorCode.CustomerNotFound
                ));
            }


            Expression<Func<Property, bool>> filter = property => property.Owner.Id == request.OwnerId && property.IsDeleted == false;
            var properties = await _propertyRepository.GetAllAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                filter,
                orderBy: q => q.OrderBy(p => p.Title), // Correct way to pass OrderBy
                includes: new Expression<Func<Property, object>>[] { p => p.Category, p => p.Owner.Person, p => p.PropertyImages,
                    p => p.Ratings}
                    );


            var totalCount = await _propertyRepository.CountAsync();

            var date = new PaginationResponse<PropertyDTO>
            {
                Items = _mapper.Map<List<PropertyDTO>>(properties),
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize,
                TotalCount = totalCount
            };
            foreach (var item in date.Items)
            {
                for (int i = 0; i < item.Images.Count; i++)
                {
                    item.Images[i] = _fileManager.GetPublicURL(item.Images[i]);
                };
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
            var response = AppResponse<PaginationResponse<PropertyDTO>>.Success(date);



            return response;
        }
    }

}
