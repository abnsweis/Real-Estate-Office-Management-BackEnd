using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Features.Testimonials.Commands.Create;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Testimonials.Commands.Update
{
    public class UpdateTestimonialCommand : IRequest<AppResponse>
    {
        public Guid? TestimonialId { get; }
        public string? RatingText { get; set; } = string.Empty;
        public int? RatingNumber { get; set; }

        public UpdateTestimonialCommand(Guid? testimonialId, string? ratingText, int? ratingNumber)
        {
            TestimonialId = testimonialId;
            RatingText = ratingText;
            RatingNumber = ratingNumber;
        }

        public class UpdateTestimonialCommandHandler : IRequestHandler<UpdateTestimonialCommand, AppResponse>
        {
            private readonly ITestimonialsRepository _testimonialsRepository;
            private readonly IUserRepository _userRepository;
            private readonly ICurrentUserService _currentUser;
            private Guid _currentUserId { get; set; }

            public UpdateTestimonialCommandHandler(
                ITestimonialsRepository testimonialsRepository,
                IUserRepository userRepository,
                ICurrentUserService currentUser)
            {
                _testimonialsRepository = testimonialsRepository;
                _userRepository = userRepository;
                _currentUser = currentUser;
            }

            public async Task<AppResponse> Handle(UpdateTestimonialCommand request, CancellationToken cancellationToken)
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

                var testimonial = await _testimonialsRepository.GetByIdAsync(request.TestimonialId.Value);

                if (testimonial == null)
                {
                    return AppResponse.Fail(new NotFoundError("Testimonial","TestimonialId",$"Not Found Testimonial With Id {request.TestimonialId.Value}",enApiErrorCode.TestimonialAlreadyExists));
                }



                testimonial.RatingText = request.RatingText;
                testimonial.RatingNumber = (byte)request.RatingNumber;

                await _testimonialsRepository.UpdateAsync(testimonial);
                await _testimonialsRepository.SaveChangesAsync(cancellationToken);

                return AppResponse.Success();
            }


            private Result _ValideteRentalData(UpdateTestimonialCommand request)
            {

                List<Error> errors = new List<Error>();

                if (!request.TestimonialId.HasValue)
                {
                    errors.Add(new ValidationError("TestimonialId", "TestimonialId is required.", enApiErrorCode.RequiredField));
                }
                if (request.TestimonialId.Value != _currentUserId)
                {
                    errors.Add(new ConflictError("UserId", "You are not allowed to edit this testimonial.", enApiErrorCode.Forbidden));
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
}
