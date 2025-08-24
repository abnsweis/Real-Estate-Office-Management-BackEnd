using Microsoft.AspNetCore.Http;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Interfaces
{
    public interface IUserDTO
    {
        string fullName   { get; }  
        IFormFile? imageURL  { get; }  
        string phoneNumber  { get; }  
        Gender gender  { get; }  
        DateOnly? dateOfBirth  { get; }  
        string email  { get; }  
        string username  { get; }  
    }
}
