using FluentValidation;
using RealEstate.Application.Common;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Features.Sales.Commands
{
    public class CreateSaleValidator : AbstractValidator<CreateSaleCommand>
    {

        public CreateSaleValidator()
        {

            RuleFor(s => s.Data.BuyerId)
                .NotEmpty()
                    .WithMessage("Buyer Id is Required")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                .Must(s => Utils.isGuid(s))
                    .WithMessage("Invalid GUID format")
                    .WithErrorCode(enApiErrorCode.InvalidGuid.ToString())
                    .OverridePropertyName("Buyer");

            RuleFor(s => s.Data.PropertyId)
                .NotEmpty()
                    .WithMessage("Property Id is Required")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                .Must(s => Utils.isGuid(s))
                    .WithMessage("Invalid GUID format")
                    .WithErrorCode(enApiErrorCode.InvalidGuid.ToString())
                    .OverridePropertyName("Property");
            RuleFor(s => s.Data.ContractImage)
                .NotEmpty()
                    .WithMessage("Contract Image is required")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                    .OverridePropertyName("ContractImage");






        }

    }
}
