using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Common.Errors
{
    public class NotFoundError : DomainError
    {
        public string EntityName { get; }
        public string PropertyName { get; }
        public string EntityErrorValue { get; }
        public NotFoundError(string entituName, string propertyName, string entityErrorValue, enApiErrorCode errorCode) : base($"{entituName} with {propertyName} {entityErrorValue} was not found", 404, errorCode)
        {
            EntityName = entituName;
            EntityErrorValue = entityErrorValue;
        }
    }
}
