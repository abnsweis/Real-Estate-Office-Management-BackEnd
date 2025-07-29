using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Common.Errors
{
    public class UnauthorizedError : DomainError
    {
        public UnauthorizedError(string message) : base(message,401,enApiErrorCode.Unauthorized)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
