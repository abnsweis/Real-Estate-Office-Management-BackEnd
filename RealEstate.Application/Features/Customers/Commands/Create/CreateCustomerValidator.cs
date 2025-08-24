using FluentValidation;
using RealEstate.Application.Dtos.Customer;
using RealEstate.Application.Dtos.Interfaces;
using RealEstate.Application.Features.Customers.Commands;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Customers.Commands.Create
{
    public class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
    {

        public CreateCustomerValidator()
        {


            RuleFor(Customer => Customer.Data.fullName)
                        .NotEmpty()
                             .WithMessage("Full Name is required.")
                             .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                        .MinimumLength(3)
                             .WithMessage("Full Name must be at least 3 characters.")
                             .WithErrorCode(enApiErrorCode.MinimumLengthViolated.ToString())
                        .MaximumLength(100)
                             .WithMessage("Full Name cannot exceed 100 characters.")
                             .WithErrorCode(enApiErrorCode.MaximumLengthExceeded.ToString())
                            .OverridePropertyName("fullName");
            RuleFor(Customer => Customer.Data.nationalId)
                .NotEmpty()
                     .WithMessage("NationallD is required.")
                        .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                .Matches(@"^\d+$")
                    .WithMessage("National ID must contain digits only.")
                    .WithErrorCode(enApiErrorCode.InvalidFormat.ToString())
                .MinimumLength(11)
                    .WithMessage("NationallD must be at least 11 characters.")
                    .WithErrorCode(enApiErrorCode.MinimumLengthViolated.ToString())
                .MaximumLength(11)
                    .WithMessage("NationallD cannot exceed 11 characters.")
                    .WithErrorCode(enApiErrorCode.MaximumLengthExceeded.ToString())
                            .OverridePropertyName("nationalId");  


            RuleFor(Customer => Customer.Data.phoneNumber)
               .NotEmpty()
                    .WithMessage("Phone is required.")
                       .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                .Matches(@"^\+?\d+$")
                    .WithMessage("Phone number must be numeric and may start with '+'.")
                    .WithErrorCode(enApiErrorCode.InvalidFormat.ToString())
                .MinimumLength(9)
                    .WithMessage("Phone number must be at least 9 digits.")
                    .WithErrorCode(enApiErrorCode.MinimumLengthViolated.ToString())
                .MaximumLength(15)
                    .WithMessage("Phone number must not exceed 15 digits.")
                    .WithErrorCode(enApiErrorCode.MaximumLengthExceeded.ToString())
                            .OverridePropertyName("phoneNumber");


            RuleFor(Customer =>  Customer.Data.gender)
                .NotNull()
                .IsInEnum()
                .WithMessage("Gender value is invalid.")
                .WithErrorCode(enApiErrorCode.InValidGender.ToString());
            RuleFor(Customer =>  Customer.Data.customerType)
                .NotNull()
                .IsInEnum()
                .WithMessage("Customer Type value is invalid.")
                .WithErrorCode(enApiErrorCode.InValidCustomerType.ToString())
                            .OverridePropertyName("gender");

            RuleFor(Customer => Customer.Data.dateOfBirth)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Date of Birth is required.")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                .Must(BeAValidDateOnly)
                    .WithMessage("Date of Birth must be a valid date.")
                    .WithErrorCode(enApiErrorCode.InvalidDate.ToString())
                .Must(dob => _GetAge(DateOnly.Parse(dob)) >= 18)
                    .WithMessage("You must be at least 18 years old.")
                    .WithErrorCode(enApiErrorCode.MinimumAgeViolated.ToString())
                .Must(dob => _GetAge(DateOnly.Parse(dob)) <= 120)
                    .WithMessage("Age must not exceed 120 years.")
                .WithErrorCode(enApiErrorCode.MaximumAgeViolated.ToString())
                            .OverridePropertyName("dateOfBirth");


        }

        private bool BeAValidDateOnly(string value)
        {
            return DateOnly.TryParse(value, out _);
        }
        private int _GetAge(DateOnly DateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - DateOfBirth.Year;

            if (DateOfBirth > today.AddYears(-age))
            {
                --age;
            }

            return age;
        }
    }
}
