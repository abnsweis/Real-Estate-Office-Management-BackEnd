using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.People.Commands
{
    public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
    {
        public CreatePersonCommandValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(1);
            RuleFor(x => x.Phone).NotEmpty().MaximumLength(22);
             
        }
    }
}
