using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Features.Rentals.Commands;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Testimonials.Commands.Create
{
    public class CreateTestimonialCommand : IRequest<AppResponse>
    { 
        public string? RatingText { get; set; } = string.Empty;
        public int? RatingNumber { get; set; }

        public CreateTestimonialCommand(string? ratingText, int? ratingNumber)
        {
            RatingText = ratingText;
            RatingNumber = ratingNumber;
        }
    }

    public class CreateTestimonialCommandHandler : IRequestHandler<CreateTestimonialCommand, AppResponse>
    {
        private readonly ITestimonialsRepository _testimonialsRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUser;
        private Guid _currentUserId { get; set; }

        public CreateTestimonialCommandHandler(
            ITestimonialsRepository testimonialsRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUser)
        {
            _testimonialsRepository = testimonialsRepository;
            _userRepository = userRepository;
            _currentUser = currentUser;
        }

        public async Task<AppResponse> Handle(CreateTestimonialCommand request, CancellationToken cancellationToken)
        {
            //if (!_currentUser.UserId.HasValue)
            //{
            //    return AppResponse.Fail(new UnauthorizedError("User is not authenticated."));
            //}

            _currentUserId = _currentUser.UserId.Value;
            var validationResults = _ValideteRentalData(request);

            if (validationResults.IsFailed)
            {
                return AppResponse.Fail(validationResults.Errors);
            }

             


            var testimonial = new Testimonial
            {
                UserId = _currentUserId,
                RatingText = request.RatingText,
                RatingNumber = (byte)request.RatingNumber
            };

            await _testimonialsRepository.AddAsync(testimonial, cancellationToken);
            await _testimonialsRepository.SaveChangesAsync(cancellationToken);

            return AppResponse.Success(testimonial.Id);
        }


        private Result _ValideteRentalData(CreateTestimonialCommand request)
        {

            List<Error> errors = new List<Error>(); 

            if (_testimonialsRepository.HasSubmittedTestimonial(_currentUserId))
            {
                errors.Add(new ConflictError(
                    "UserId",
                    "You have already submitted a testimonial. Thank you for your feedback!",
                    enApiErrorCode.TestimonialAlreadyExists));
            }

            if (string.IsNullOrEmpty(request.RatingText))
            {
                errors.Add(new ValidationError("RatingText", "Rating text is required.", enApiErrorCode.RequiredField));
            }
            if (request.RatingNumber is null)
            {
                errors.Add(new ValidationError("RatingNumber", "Rating Number is required.", enApiErrorCode.RequiredField));
            }
            if (request.RatingNumber > 5)
            {
                errors.Add(new ValidationError("RatingNumber", "Rating number cannot be greater than 5.", enApiErrorCode.MaximumLengthExceeded));
            }

            if (request.RatingNumber < 1)
            {
                errors.Add(new ValidationError("RatingNumber", "Rating number cannot be less than 1.", enApiErrorCode.MinimumLengthViolated));
            }



            return errors.Any() ? Result.Fail(errors) : Result.Ok();
        }
    }
}
