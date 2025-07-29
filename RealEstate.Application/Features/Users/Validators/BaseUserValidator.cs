using FluentValidation;
using RealEstate.Application.Dtos.Interfaces;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Users.Validators
{
    public abstract class BaseUserValidator<T> : AbstractValidator<T> where T : IUserDTO
    {


        protected void AddCommonRules()
        {
            





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
