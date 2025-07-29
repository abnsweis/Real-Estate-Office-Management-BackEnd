using Microsoft.AspNetCore.Http;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Interfaces
{
    public interface ICustomerDTO
    {
        string fullName { get; } 
        string phoneNumber { get; }
        string nationalId { get; }
        enGender gender { get; }
        enCustomerType customerType { get; set; }
        string dateOfBirth { get; } 
    }
     
}
