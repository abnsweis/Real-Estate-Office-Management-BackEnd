using FluentValidation;
using RealEstate.Application.Common;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Features.Sales.Commands
{
    public class CreateSaleValidator : AbstractValidator<CreateSaleCommand>
    {

        public CreateSaleValidator()
        {

            RuleFor(s => s.Data.SellerId)
                .NotEmpty()
                    .WithMessage("Seller Id is Required")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                .Must(s => Utils.isGuid(s))
                    .WithMessage("Invalid GUID format")
                    .WithErrorCode(enApiErrorCode.InvalidGuid.ToString())
                    .OverridePropertyName("SellerId");

            RuleFor(s => s.Data.BuyerId)
                .NotEmpty()
                    .WithMessage("Buyer Id is Required")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                .Must(s => Utils.isGuid(s))
                    .WithMessage("Invalid GUID format")
                    .WithErrorCode(enApiErrorCode.InvalidGuid.ToString())
                    .OverridePropertyName("BuyerId");

            RuleFor(s => s.Data.PropertyId)
                .NotEmpty()
                    .WithMessage("Property Id is Required")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                .Must(s => Utils.isGuid(s))
                    .WithMessage("Invalid GUID format")
                    .WithErrorCode(enApiErrorCode.InvalidGuid.ToString())
                    .OverridePropertyName("PropertyId");

            RuleFor(s => s.Data.Price)
                    .NotEmpty()
                    .WithMessage("Price is Required")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())

                .GreaterThan(0)
                    .WithMessage("Price must be greater than 0")
                    .WithErrorCode(enApiErrorCode.RequiredGreaterThanZero.ToString()).OverridePropertyName("Price");
            RuleFor(s => s.Data.SaleDate)
                .NotEmpty()
                    .WithMessage("Sale date is required.")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                .Must(s => DateOnly.TryParse(s, out _))
                    .WithMessage("Invalid sale date format.")
                    .WithErrorCode(enApiErrorCode.InvalidDate.ToString());

            RuleFor(s => s.Data.ContractImage)
                .NotEmpty()
                    .WithMessage("Contract Image is required")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                    .OverridePropertyName("Images");






        }

    }
}
