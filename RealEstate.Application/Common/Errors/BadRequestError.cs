using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Common.Errors
{
    public class BadRequestError : DomainError
    {
        public string PropertyName { get; }

        public BadRequestError(string propertyName, string message, enApiErrorCode errorCode) : base(message, 400, errorCode)
        {
            PropertyName = propertyName;
        }

    }
}
