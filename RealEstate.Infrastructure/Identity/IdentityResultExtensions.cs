using FluentResults;
using Microsoft.AspNetCore.Identity;
using RealEstate.Infrastructure.Identity.Attributes;
using RealEstate.Infrastructure.Identity.IdentiyErrors;
using RealEstate.Infrastructure.Identity.IdentiyErrors.Enums;
using System.Reflection;

namespace RealEstate.Infrastructure.Identity;
public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        var err = Result.Fail(result.Errors.Select(e => IdentityErrorService.HandleError(e)).Cast<IError>().ToList());

        return result.Succeeded
            ? Result.Ok()
            : err;
    }


    public static string GetDescription(this enIdentityErrorCode errorCode)
    {
        var field = errorCode.GetType().GetField(errorCode.ToString());
        var attribute = field.GetCustomAttribute<ErrorDescriptionAttribute>();
        return attribute?.Description ?? errorCode.ToString();
    }

    public static enHttpStatusCode GetStatusCode(this enIdentityErrorCode errorCode)
    {
        var field = errorCode.GetType().GetField(errorCode.ToString());
        var attribute = field.GetCustomAttribute<HttpStatusCodeAttribute>();
        return attribute?.StatusCode ?? enHttpStatusCode.BadRequest;
    }


    public static enErrorCategory GetCategory(this enIdentityErrorCode errorCode)
    {
        var field = errorCode.GetType().GetField(errorCode.ToString());
        return field?.GetCustomAttribute<ErrorCategoryAttribute>()?.Category ?? enErrorCategory.Default;
    }
}
