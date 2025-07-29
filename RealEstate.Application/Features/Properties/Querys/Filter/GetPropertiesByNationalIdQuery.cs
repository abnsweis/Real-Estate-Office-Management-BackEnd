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

    public class GetPropertiesByNationalIdQuery : IRequest<AppResponse<PaginationResponse<PropertyDTO>>>
    {
        public string? NationalId { get; }

        public PaginationRequest Pagination { get; set; }

        public GetPropertiesByNationalIdQuery(PaginationRequest pagination, string? nationalId)
        {
            Pagination = pagination;
            NationalId = nationalId;
        }
    }


    public class GetPropertiesByNationalIdQueryHndler : IRequestHandler<GetPropertiesByNationalIdQuery, AppResponse<PaginationResponse<PropertyDTO>>>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IFileManager _fileManager;
        public GetPropertiesByNationalIdQueryHndler(
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

        public async Task<AppResponse<PaginationResponse<PropertyDTO>>> Handle(GetPropertiesByNationalIdQuery request, CancellationToken cancellationToken)
        {

            if (!_customerRepository.IsCustomerExists(request.NationalId))
            {
                return AppResponse<PaginationResponse<PropertyDTO>>.Fail(new NotFoundError(
                    "Owner",
                    "NationalId",
                    request.NationalId,
                    enApiErrorCode.CustomerNotFound
                ));
            }


            Expression<Func<Property, bool>> filter = property => property.Owner.Person.NationalId == request.NationalId && property.IsDeleted == false;
            var properties = await _propertyRepository.GetAllAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                filter,
                orderBy: q => q.OrderBy(p => p.Title), // Correct way to pass OrderBy
                includes: new Expression<Func<Property, object>>[] { p => p.Category, p => p.Owner.Person, p => p.PropertyImages,
                    p => p.Ratings }
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
