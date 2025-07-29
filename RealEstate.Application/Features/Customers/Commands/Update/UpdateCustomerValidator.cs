using RealEstate.Application.Features.Customers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Customers.Commands.Update
{
    public class UpdateCustomerValidator : BaseCustomerValidator<UpdateCustomerCommand>
    {

        public UpdateCustomerValidator()
        {
            AddCommonRules();
        }
    }
}
