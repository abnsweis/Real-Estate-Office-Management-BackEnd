using FluentValidation;
using RealEstate.Application.Common;
using RealEstate.Application.Features.Properties.Commands;
using RealEstate.Application.Features.Propertys.Commands.Update;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Features.Customers.Commands.Update
{
    public class UpdatePropertyValidator : AbstractValidator<UpdatePropertyCommand>
    {

        public UpdatePropertyValidator()
        {
            RuleFor(cmd => cmd.Data.Title)
                .NotEmpty()
                    .WithMessage("Title Field is Required")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                    .OverridePropertyName("Title");
            RuleFor(cmd => cmd.Data.CategoryId)
                .NotEmpty()
                    .WithMessage("Category Id Field is Required")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                .Must(p => Utils.isGuid(p))
                    .WithMessage("Invalid GUID format")
                    .WithErrorCode(enApiErrorCode.InvalidGuid.ToString())
                    .OverridePropertyName("CategoryId");

            RuleFor(cmd => cmd.Data.Price)
                .NotEmpty()
                    .WithMessage("Price is required")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                .GreaterThan(0)
                    .WithMessage("Price must be greater than 0")
                    .WithErrorCode(enApiErrorCode.RequiredGreaterThanZero.ToString())
                    .OverridePropertyName("Price");

            RuleFor(cmd => cmd.Data.Location)
                .NotEmpty()
                    .WithMessage("Location Field is Required")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString()).OverridePropertyName("Location");

            RuleFor(cmd => cmd.Data.Address)
                .NotEmpty()
                    .WithMessage("Address Field is Required")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString()).OverridePropertyName("Address");

            RuleFor(cmd => cmd.Data.Area)
                .NotEmpty()
                    .WithMessage("Area is required")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                .GreaterThan(0)
                    .WithMessage("Area must be greater than 0")
                    .WithErrorCode(enApiErrorCode.RequiredGreaterThanZero.ToString()).OverridePropertyName("Area");

            RuleFor(cmd => cmd.Data.Images)
                .NotEmpty()
                    .WithMessage("Image is required")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                .Must(list => list.Count > 0)
                    .WithMessage("You must upload at least one image")
                    .WithErrorCode(enApiErrorCode.MinimumLengthViolated.ToString()).OverridePropertyName("Images");
        }
    }
}
