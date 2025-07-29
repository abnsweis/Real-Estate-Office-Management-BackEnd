using FluentValidation;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Auth.Commands.Login
{
    public class LoginValidator : AbstractValidator<LoginCommand>
    {

        public LoginValidator()
        {
            RuleFor(lf => lf.LoginInfo.username).NotEmpty().WithMessage("username is required")
                .WithErrorCode(enApiErrorCode.RequiredField.ToString())
                .OverridePropertyName("username");
            RuleFor(lf => lf.LoginInfo.password).NotEmpty()
                .WithMessage("password is required").WithErrorCode(enApiErrorCode.RequiredField.ToString())
                .OverridePropertyName("password");
        }
    }
}
