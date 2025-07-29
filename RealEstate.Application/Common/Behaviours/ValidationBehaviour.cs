using FluentValidation;
using MediatR;
using RealEstate.Application.Common.Errors;
using FluentResults;
using RealEstate.Domain.Enums;
using RealEstate.Application.Dtos.ResponseDTO;

namespace RealEstate.Application.Common.Behaviours
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken))
                );

                var failures = validationResults
                    .Where(r => r.Errors.Any())
                    .SelectMany(r => r.Errors)
                    .ToList();

                if (failures.Any())
                {
                    var errors = failures
                        .Select(f =>
                        {
                            if (!Enum.TryParse<enApiErrorCode>(f.ErrorCode, out var errorCode))
                            {
                                errorCode = enApiErrorCode.Unknown;
                            }

                            return new ValidationError(f.PropertyName, f.ErrorMessage, errorCode);
                        })
                        .Cast<IError>()
                        .ToList();



                    if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(AppResponse<>))
                    {
                        var dataType = typeof(TResponse).GetGenericArguments()[0];

                        var method = typeof(AppResponse<>)
                            .MakeGenericType(dataType)
                            .GetMethod("Fail", new[] { typeof(List<IError>) });

                        var failedAppResponse = method!.Invoke(null, new object[] { errors });

                        return (TResponse)failedAppResponse!;
                    }
                }
            }

            return await next();
        }



        //public enApiErrorCode GetErrorCode()
    }
}
