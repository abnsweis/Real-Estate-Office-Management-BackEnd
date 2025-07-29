using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Common.Errors
{
    public class ConflictError : DomainError
    {

        public ConflictError(string propertyName, string message, enApiErrorCode errorCode) : base(message, 409, errorCode)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }
    }
}
