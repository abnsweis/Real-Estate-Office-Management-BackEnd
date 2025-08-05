using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.CustomerDTO
{
    public class CustomerDTO  
    {    
        public string CustomerId { get; set; }  
        public string FullName { get; set; }  

        public string NationalId { get; set; }  
        public string PhoneNumber { get; set; }  

        public string DateOfBirth { get; set; }

        public string Gender { get; set; }  

        public string customerType { get; set; } 
        public string ImageURL { get; set; } 
    }
}
