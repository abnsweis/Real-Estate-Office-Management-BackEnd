using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.Ratings;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Features.Ratings.Commands.Create
{
    public class CreateRatingCommand : IRequest<AppResponse<Guid>>
    { 
        public CreateUpdateRatingDTO Data { get; }
        public string? PropertyId { get; }

        public CreateRatingCommand(CreateUpdateRatingDTO Data,string? propertyId) {
            this.Data = Data;
            PropertyId = propertyId;
        }

    }


    public class CreateRatingCommandHandler : IRequestHandler<CreateRatingCommand, AppResponse<Guid>>
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly ICurrentUserService _user;

        public CreateRatingCommandHandler(
            IRatingRepository ratingRepository,
            IUserRepository userRepository,
            IPropertyRepository propertyRepository,
            ICurrentUserService user

            )
        {
            this._ratingRepository = ratingRepository;
            this._userRepository = userRepository;
            this._propertyRepository = propertyRepository;
            this._user = user;
        }

        public async Task<AppResponse<Guid>> Handle(CreateRatingCommand request, CancellationToken cancellationToken)
        {
            var validationResults = _ValideteRentalData(request);

            if (validationResults.IsFailed)
            {
                return AppResponse<Guid>.Fail(validationResults.Errors);
            }



            var rating = new Rating
            {
                RatingNumber = Convert.ToByte(request.Data.RatingNumber),
                RatingText = request.Data.RatingText,
                UserId = _user.UserId.Value,
                PropertyId = Guid.Parse(request.PropertyId)
            };



            await _ratingRepository.AddAsync(rating);
            await _ratingRepository.SaveChangesAsync();

            return AppResponse<Guid>.Success(rating.Id);
        }


        private Result _ValideteRentalData(CreateRatingCommand request)
        {
            List<Error> errors = new();
            var isPropertyIdGuid = Guid.TryParse(request.PropertyId, out var propertyId); 



        
            if (!isPropertyIdGuid)
                errors.Add(new ValidationError("propertyId", "Invalid GUID format", enApiErrorCode.InvalidGuid));
                 
            


            if (!_userRepository.IsUserExists(_user.UserId.Value))
            {
                return Result.Fail(new ValidationError("UserId", $"Not Found user With Id {_user.UserId.Value}", enApiErrorCode.UserNotFound));
            }


            if (!_propertyRepository.IsPropertyExistsById(propertyId))
            {
                return Result.Fail(new ValidationError("propertyId", $"Not Found property With Id {request.PropertyId}", enApiErrorCode.PropertyNotFound));
            }
            if (_ratingRepository.HasRatingByUser(_user.UserId.Value, propertyId))
            {
                errors.Add(new ConflictError("UserId", $"The user is already ratined the property", enApiErrorCode.UserNotFound));
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
