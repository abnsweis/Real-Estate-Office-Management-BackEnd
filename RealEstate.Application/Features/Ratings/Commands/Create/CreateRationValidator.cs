using FluentValidation;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Features.Ratings.Commands.Create
{
    public class CreateRationValidator : AbstractValidator<CreateRatingCommand>
    {


        public CreateRationValidator() {

            RuleFor(r => r.Data.RatingText)
                .NotEmpty()
                    .WithMessage("Rating Text Is Requierd")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                    .OverridePropertyName("RatingText");


            RuleFor(r => r.Data.RatingNumber)
                .NotEmpty()
                    .WithMessage("Rating Number Is Requierd")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString()); 
        }
    }
}
