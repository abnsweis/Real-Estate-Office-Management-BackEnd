using FluentValidation;
using RealEstate.Application.Common;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Rentals.Commands
{
    public class CreateRentalValidator : AbstractValidator<CreateRentalCommand>
    {

        public CreateRentalValidator()
        { 

            RuleFor(r => r.Data.LesseeId)
                .NotEmpty()
                    .WithMessage("Lessee Id is Required")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                .Must(s => Utils.isGuid(s))
                    .WithMessage("Invalid GUID format")
                    .WithErrorCode(enApiErrorCode.InvalidGuid.ToString())
                    .OverridePropertyName("LesseeId");

            RuleFor(r => r.Data.PropertyId)
                .NotEmpty()
                    .WithMessage("Property Id is Required")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                .Must(s => Utils.isGuid(s))
                    .WithMessage("Invalid GUID format")
                    .WithErrorCode(enApiErrorCode.InvalidGuid.ToString())
                    .OverridePropertyName("PropertyId");

            RuleFor(r => r.Data.RentPrice)
                    .NotEmpty()
                    .WithMessage("Rent Price is Required")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())

                .GreaterThan(0)
                    .WithMessage("Price must be greater than 0")
                    .WithErrorCode(enApiErrorCode.RequiredGreaterThanZero.ToString()).OverridePropertyName("RentPrice");
            RuleFor(r => r.Data.StartDate)
                .NotEmpty()
                    .WithMessage("Start Date is required.")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                    .Must(dt => dt.HasValue && dt.Value >= DateTime.Today)
                        .WithMessage("Start date must be today or later.")

                    .WithErrorCode(enApiErrorCode.InvalidDate.ToString())
                    .OverridePropertyName("StartDate");

            RuleFor(r => r.Data.Duration)
                .NotEmpty()
                    .WithMessage("Duration is required.")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                .GreaterThan(0)
                    .WithMessage("Duration must be greater than 0.")
                    .WithErrorCode(enApiErrorCode.RequiredGreaterThanZero.ToString())
                    .OverridePropertyName("Duration");


            RuleFor(r => r.Data.ContractImageUrl)
                .NotEmpty()
                    .WithMessage("Contract Image is required")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                    .OverridePropertyName("Images");


            RuleFor(r => r.Data.RentType)
                .IsInEnum()
                    .WithMessage("Invalid RentType value.")
                    .WithErrorCode(enApiErrorCode.InvalidEnumValue.ToString())
                .OverridePropertyName("RentType");





        }
    }
}
