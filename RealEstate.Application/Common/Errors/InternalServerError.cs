using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Common.Errors
{
    public class InternalServerError : DomainError
    {
        public string Key { get; }

        public InternalServerError(string Key, string message, enApiErrorCode errorCode) : base(message, 500, errorCode)
        {
            this.Key = Key;
        }

    }
}
