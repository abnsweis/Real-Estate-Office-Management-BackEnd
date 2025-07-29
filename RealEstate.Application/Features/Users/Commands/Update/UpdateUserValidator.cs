using FluentValidation;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Dtos.Interfaces;
using RealEstate.Application.Features.Users.Validators;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Features.Users.Commands.Update
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
    {

        public UpdateUserValidator()
        {

            RuleFor(user => user.Data.FullName)
                .NotEmpty()
                     .WithMessage("Full Name is required.")
                     .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                .MinimumLength(3)
                     .WithMessage("Full Name must be at least 3 characters.")
                     .WithErrorCode(enApiErrorCode.MinimumLengthViolated.ToString())
                .MaximumLength(100)
                     .WithMessage("Full Name cannot exceed 100 characters.")
                     .WithErrorCode(enApiErrorCode.MaximumLengthExceeded.ToString());



            RuleFor(user => user.Data.Username)
                          .NotEmpty()
                               .WithMessage("Username is required.")
                                  .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                           .MinimumLength(4)
                               .WithMessage("Username number must be at least 4 characters.")
                               .WithErrorCode(enApiErrorCode.MinimumLengthViolated.ToString());

            RuleFor(user => user.Data.Email)
                    .NotEmpty()
                         .WithMessage("Email is required.")
                         .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                    .EmailAddress()
                        .WithMessage("Invalid email format.")
                        .WithErrorCode(enApiErrorCode.InvalidFormat.ToString());

            RuleFor(user => user.Data.PhoneNumber)
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
                    .WithErrorCode(enApiErrorCode.MaximumLengthExceeded.ToString());

            RuleFor(user => user.Data.Gender)
                .NotNull()
                .IsInEnum()
                .WithMessage("Gender value is invalid.")
                .WithErrorCode(enApiErrorCode.InValidGender.ToString());


            RuleFor(user => user.Data.DateOfBirth)
                .NotEmpty().WithMessage("Date of Birth is required.")
                    .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                .Must(dob => dob <= DateOnly.FromDateTime(DateTime.UtcNow))
                    .WithMessage("Date of Birth must be in the past.")
                    .WithErrorCode(enApiErrorCode.InvalidDate.ToString())
                .Must(dob => _GetAge(dob.Value) >= 18)
                    .WithMessage("You must be at least 18 years old.")
                    .WithErrorCode(enApiErrorCode.InvalidDate.ToString())
                .Must(dob => _GetAge(dob.Value) <= 120)
                    .WithMessage("Invalid age.")
                .WithErrorCode(enApiErrorCode.InvalidDate.ToString());


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







