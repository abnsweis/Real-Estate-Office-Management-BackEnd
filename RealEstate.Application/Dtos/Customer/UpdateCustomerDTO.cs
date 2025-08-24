using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Customer
{
    public class UpdateCustomerDTO : BaseCustomerDTO
    {
        private Guid? _customerId { get; set; }
        
         

        public Guid? GetCustomerId()
        {
            return this._customerId;

        }
        public void setCustomerId(Guid? customerId)
        {
            this._customerId = customerId;
        }
    }
}
