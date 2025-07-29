using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Ratings.Querys
{
    public class IsRatingByUserQuery : IRequest<AppResponse<bool>>
    {
        public IsRatingByUserQuery(Guid? propertyId)
        {
            PropertyId = propertyId;
        }

        public Guid? PropertyId { get; }
    }

    public class IsRatingByUserQueryHandler : IRequestHandler<IsRatingByUserQuery, AppResponse<bool>>
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly ICurrentUserService _currentUserService;

        public IsRatingByUserQueryHandler(IRatingRepository ratingRepository,ICurrentUserService currentUserService )
        {
            this._ratingRepository = ratingRepository;
            this._currentUserService = currentUserService;
        }

        public async Task<AppResponse<bool>> Handle(IsRatingByUserQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.UserId.HasValue)   
            {
                return AppResponse<bool>.Fail(new UnauthorizedError("UserId in Null"));
            }
            if (!request.PropertyId.HasValue)
            {
                return AppResponse<bool>.Fail(new NotFoundError("property","PropertyId",request.PropertyId.ToString(),enApiErrorCode.PropertyNotFound));
            }
            var isRating = _ratingRepository.HasRatingByUser(_currentUserService.UserId!.Value,request.PropertyId.Value);
            return AppResponse<bool>.Success(isRating);
        }
    }
}
