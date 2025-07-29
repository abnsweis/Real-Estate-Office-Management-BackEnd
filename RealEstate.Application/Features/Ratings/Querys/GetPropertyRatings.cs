using AutoMapper;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Dtos.Ratings;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Dtos.Sales;
using RealEstate.Domain.Enums;
using System.Linq.Expressions;

namespace RealEstate.Application.Features.Ratings.Querys
{
    public class GetPropertyRatingsQuery : IRequest<AppResponse<PaginationResponse<RatingDTO>>>
    {
        public Guid? PropertyId { get; }
        public PaginationRequest Pagination { get; }
        public GetPropertyRatingsQuery(Guid? propertyId, PaginationRequest pagination)
        {
            PropertyId = propertyId;
            Pagination = pagination;
        }

    }


    public class GetPropertyRatingsQueryHandler : IRequestHandler<GetPropertyRatingsQuery, AppResponse<PaginationResponse<RatingDTO>>>
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IFileManager _fileManager;
        private readonly IUserRepository _userRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;

        public GetPropertyRatingsQueryHandler(
                    IRatingRepository ratingRepository,
                    IFileManager fileManager,
                    IUserRepository userRepository,
                    IPropertyRepository propertyRepository,
                    IMapper mapper

            )
        {
            this._ratingRepository = ratingRepository;
            _fileManager = fileManager;
            this._userRepository = userRepository;
            this._propertyRepository = propertyRepository;
            _mapper = mapper;
        }

        public async Task<AppResponse<PaginationResponse<RatingDTO>>> Handle(GetPropertyRatingsQuery request, CancellationToken cancellationToken)
        {


            if (!_propertyRepository.IsPropertyExistsById(request.PropertyId.Value))
            {
                return AppResponse<PaginationResponse<RatingDTO>>.Fail(new ValidationError("propertyId", $"Not Found property With Id {request.PropertyId}", enApiErrorCode.PropertyNotFound));
            }


            var ratings = await _ratingRepository.GetAllAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                filter : r => r.PropertyId == request.PropertyId,
                includes : new Expression<Func<Domain.Entities.Rating, Object>>[]{r => r.Property}
                );


            var rationsDTO = _mapper.Map<List<RatingDTO>>(ratings);
            var rationsWithUsersInfo = await _includeUsersAsync(rationsDTO);
            // Get total count of properties (after filtering)
            var totalCount = await _ratingRepository.CountAsync();

            // Create paginated response
            var paginatedResponse = new PaginationResponse<RatingDTO>
            {
                Items = rationsWithUsersInfo.ToList(),
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize,
                TotalCount = totalCount
            };
 
            // Return successful response with the paginated data
            var response = AppResponse<PaginationResponse<RatingDTO>>.Success(paginatedResponse);
            return response;
        }

        private async Task<IEnumerable<RatingDTO>> _includeUsersAsync(IEnumerable<RatingDTO> ratings)
        {
            foreach (var item in ratings)
            {
                if (Guid.TryParse(item.UserId, out var userId))
                {
                    var user = await _userRepository.FirstOrDefaultAsync(u => u.Id == Guid.Parse(item.UserId) ,Includes : u=>u.Person);

                    if (user != null)
                    {
                        item.FullName = user.Person.FullName;
                        item.Username = user.UserName;
                        item.ImageURL = _fileManager.GetPublicURL(user.Person.ImageURL);
                    }
                }
            }

            return ratings;
        }
    }
}
