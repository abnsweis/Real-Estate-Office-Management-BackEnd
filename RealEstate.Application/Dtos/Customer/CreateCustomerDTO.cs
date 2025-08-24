using RealEstate.Application.Dtos.Interfaces;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Customer
{
    public class CreateCustomerDTO  
    {
        public string? fullName { get; set; }  
        public string? phoneNumber { get; set; }
        public string? nationalId { get; set; }
        public Gender? gender { get; set; }
        public CustomerType? customerType { get; set; }
        public string? dateOfBirth { get; set; }

    }
}
