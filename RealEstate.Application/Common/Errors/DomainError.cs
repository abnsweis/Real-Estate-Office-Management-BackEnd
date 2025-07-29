using FluentResults;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Common.Errors
{
    public abstract class DomainError : Error
    {
        public int StatusCode { get; }
        public enApiErrorCode ErrorCode { get; }

        public DomainError(string message, int statusCode, enApiErrorCode errorCode) : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Metadata.Add(nameof(StatusCode), statusCode);
            Metadata.Add(nameof(ErrorCode), errorCode.ToString());
        }
    }
}
