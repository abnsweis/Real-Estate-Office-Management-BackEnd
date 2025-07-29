using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Common.Errors;
using RealEstate.Infrastructure.Identity.IdentiyErrors;

namespace RealEstate.API.Transformers
{
    /// <summary>
    /// Transforms FluentResults failures into ProblemDetails responses for ASP.NET Core APIs.
    /// Handles specific error types like NotFoundError and ValidationError with appropriate HTTP status codes.
    /// </summary>
    public class CustomAspNetCoreResultEndpointProfile : DefaultAspNetCoreResultEndpointProfile
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomAspNetCoreResultEndpointProfile(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        public override ActionResult TransformFailedResultToActionResult(FailedResultToActionResultTransformationContext context)
        {
            var result = context.Result;

            if (result.HasError<NotFoundError>(out var notFoundErrors))
            {
                if (notFoundErrors.FirstOrDefault() != null)
                    return HandleNotFoundError(notFoundErrors.First());
            }

            if (result.HasError<ValidationError>(out var validationErrors))
            {
                return HandleValidationErrors(validationErrors);
            }

            if (result.HasError<BadRequestError>(out var badRequestErrors))
            {
                return HandleBadRequestErrors(badRequestErrors);
            }

            if (result.HasError<CustomIdentityError>(out var identityErrors))
            {
                return HandleIdentityErrors(identityErrors);
            }

            if (result.HasError<ConflictError>(out var conflictErrors))
            {
                return HandleConflictErrors(conflictErrors);
            }

            return HandleInternalServerError(result.Errors.Cast<InternalServerError>());
        }

        // -------------------------------
        // Shared Builder for ProblemDetails
        // -------------------------------

        private ProblemDetails BuildProblemDetails(
            int statusCode,
            string title,
            string type,
            string? detail = null,
            object? errors = null)
        {
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = type,
                Detail = detail,
                Instance = _httpContextAccessor.HttpContext?.Request.Path
            };

            problemDetails.Extensions["traceId"] = _httpContextAccessor?.HttpContext?.TraceIdentifier;

            if (errors != null)
                problemDetails.Extensions["errors"] = errors;

            return problemDetails;
        }

        // -------------------------------
        // Error Handlers
        // -------------------------------

        private ActionResult HandleNotFoundError(NotFoundError error)
        {
            var problemDetails = BuildProblemDetails(
                StatusCodes.Status404NotFound,
                $"Not Found {error.EntityName}",
                "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                detail: error.Message,
                errors: new { errorCode = error.ErrorCode.ToString() }
            );

            return new NotFoundObjectResult(problemDetails);
        }

        private ActionResult HandleValidationErrors(IEnumerable<ValidationError> validationErrors)
        {
            var groupedErrors = validationErrors
                .GroupBy(e => e.PropertyName)
                .Select(g => new
                {
                    field = g.Key,
                    messages = g.Select(e => new
                    {
                        text = e.Message,
                        code = e.ErrorCode.ToString()
                    }).ToList()
                }).ToList();

            var problemDetails = BuildProblemDetails(
                StatusCodes.Status400BadRequest,
                "Validation Failed: One or more fields are invalid.",
                "https://tools.ietf.org/html/rfc9110#section-15.4.1",
                errors: groupedErrors
            );

            return new BadRequestObjectResult(problemDetails);
        }

        private ActionResult HandleBadRequestErrors(IEnumerable<BadRequestError> badRequestErrors)
        {
            var groupedErrors = badRequestErrors
                .GroupBy(e => e.PropertyName)
                .Select(g => new
                {
                    field = g.Key,
                    messages = g.Select(e => new
                    {
                        text = e.Message,
                        code = e.ErrorCode.ToString()
                    }).ToList()
                }).ToList();

            var problemDetails = BuildProblemDetails(
                StatusCodes.Status400BadRequest,
                "Bad Request: One or more fields are invalid.",
                "https://tools.ietf.org/html/rfc9110#section-15.4.1",
                errors: groupedErrors
            );

            return new BadRequestObjectResult(problemDetails);
        }

        private ActionResult HandleConflictErrors(IEnumerable<ConflictError> conflictErrors)
        {
            var groupedErrors = conflictErrors
                .GroupBy(e => e.PropertyName)
                .Select(g => new
                {
                    field = g.Key,
                    messages = g.Select(e => new
                    {
                        text = e.Message,
                        code = e.ErrorCode.ToString()
                    }).ToList()
                }).ToList();

            var problemDetails = BuildProblemDetails(
                StatusCodes.Status409Conflict,
                "Conflict: A record with the same value already exists.",
                "https://tools.ietf.org/html/rfc9110#section-15.4.1",
                errors: groupedErrors
            );

            return new ConflictObjectResult(problemDetails);
        }

        private ActionResult HandleIdentityErrors(IEnumerable<CustomIdentityError> identityErrors)
        {
            var groupedErrors = identityErrors
                .GroupBy(e => e.Category)
                .Select(g => new
                {
                    field = g.Key,
                    messages = g.Select(e => new
                    {
                        text = e.Message,
                        code = e.ErrorCode.ToString()
                    }).ToList()
                }).ToList();

            var problemDetails = BuildProblemDetails(
                StatusCodes.Status400BadRequest,
                "Identity Error: One or more identity validations failed.",
                "https://tools.ietf.org/html/rfc9110#section-15.4.1",
                errors: groupedErrors
            );

            return new BadRequestObjectResult(problemDetails);
        }

        private ActionResult HandleInternalServerError(IEnumerable<InternalServerError> serverErrors)
        {
            var errorList = serverErrors.Select(g => new
            {
                field = g.Key,
                messages = new
                {
                    text = g.Message,
                    code = g.ErrorCode.ToString(),
                    StatusCode = g.StatusCode
                }
            }).ToList();

            var problemDetails = BuildProblemDetails(
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                detail: "An unexpected error occurred on the server.",
                errors: errorList
            );

            return new ObjectResult(problemDetails)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}
