using FluentValidation;
using RealEstate.Application.Features.Ratings.Commands.Update;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Ratings.Commands.Update
{
    public class UpdateRationValidator : AbstractValidator<UpdateRatingCommand>
    {


        public UpdateRationValidator() {
             
            RuleFor(r => r.RatingId)
                .NotEmpty()
                    .WithMessage("Rating Id Is Requierd")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                    .OverridePropertyName("RatingId");

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
