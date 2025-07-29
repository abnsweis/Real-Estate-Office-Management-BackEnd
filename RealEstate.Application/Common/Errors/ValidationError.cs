using RealEstate.Domain.Enums;

namespace RealEstate.Application.Common.Errors
{
    public class ValidationError : DomainError
    {
        public string PropertyName { get; }
        public ValidationError(string propertyName, string message, enApiErrorCode errorCode) : base($"{message}", 400, errorCode)
        {
            PropertyName = propertyName;
        }

    }
}
