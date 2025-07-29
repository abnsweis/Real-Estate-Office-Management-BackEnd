using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.Ratings;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Features.Ratings.Commands.Create;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Ratings.Commands.Update
{
    public class UpdateRatingCommand : IRequest<AppResponse>
    {
        public CreateUpdateRatingDTO Data { get; }
        public string? RatingId { get; }

        public UpdateRatingCommand(CreateUpdateRatingDTO data,string? ratingId ) {
            Data= data;
            RatingId = ratingId;
        }

    }




    public class UpdateRatingCommandHandler : IRequestHandler<UpdateRatingCommand, AppResponse>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IRatingRepository _ratingRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _user;

        public UpdateRatingCommandHandler(IPropertyRepository propertyRepository
            ,IRatingRepository ratingRepository,
            IUserRepository userRepository,
            ICurrentUserService user
            )
        {
            this._propertyRepository = propertyRepository;
            this._ratingRepository = ratingRepository;
            this._userRepository = userRepository;
            this._user = user;
        }
        public async Task<AppResponse> Handle(UpdateRatingCommand request, CancellationToken cancellationToken)
        {
            var validationResults = _ValideteRentalData(request);

            if (validationResults.IsFailed)
            {
                return AppResponse.Fail(validationResults.Errors);
            }


            var rating = await _ratingRepository.GetByIdAsync(Guid.Parse(request.RatingId!));


            rating.RatingNumber = (byte)request.Data.RatingNumber;
            rating.RatingText = request.Data.RatingText;


            await _ratingRepository.UpdateAsync(rating);
            await _ratingRepository.SaveChangesAsync(cancellationToken);


            return AppResponse.Success();
        }

        private Result _ValideteRentalData(UpdateRatingCommand request)
        {
            List<Error> errors = new();
            var isRatingIdGuid = Guid.TryParse(request.RatingId, out var RatingId);



            if (!isRatingIdGuid)
            { 
                errors.Add(new ValidationError("isRatingIdGuid", "Invalid GUID format", enApiErrorCode.InvalidGuid));
            }


            if (!_userRepository.IsUserExists(_user.UserId.Value))
            {
                errors.Add(new ValidationError("UserId", $"Not Found user With Id {_user.UserId.Value}", enApiErrorCode.UserNotFound));
            } 

            if (request.Data.RatingNumber > 5)
            {
                errors.Add(new ValidationError("RatingNumber", "Rating number cannot be greater than 5.", enApiErrorCode.MaximumLengthExceeded));
            }

            if (request.Data.RatingNumber < 1)
            {
                errors.Add(new ValidationError("RatingNumber", "Rating number cannot be less than 1.", enApiErrorCode.MinimumLengthViolated));
            }

            return errors.Any() ? Result.Fail(errors) : Result.Ok();
        }

    }
}
