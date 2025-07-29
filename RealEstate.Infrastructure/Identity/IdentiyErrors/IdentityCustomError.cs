using RealEstate.Application.Common.Errors;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Infrastructure.Identity.IdentiyErrors
{
    public class CustomIdentityError : DomainError
    {
        public string Category; 

        public CustomIdentityError(string category, string message , int statusCode,enApiErrorCode errorCode) : base(message,statusCode, errorCode)
        {
            Category = category; 
        }
    }
}
