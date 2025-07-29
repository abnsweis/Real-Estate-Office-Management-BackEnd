using Microsoft.AspNetCore.Identity;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.Identity.IdentiyErrors.Enums;

namespace RealEstate.Infrastructure.Identity.IdentiyErrors
{
    public static class IdentityErrorService
    {
        public static CustomIdentityError HandleError(IdentityError error)
        {
            var errorCode = Enum.Parse<enIdentityErrorCode>(error.Code);
            var Category = errorCode.GetCategory();
            var Message = errorCode.GetDescription();
            var StatusCode = errorCode.GetStatusCode();

            var ApiErrorCode = _MapToApiErrorCode(errorCode);
            return new CustomIdentityError(Category.ToString(), Message, StatusCode.GetHashCode(), ApiErrorCode);

        }
        private static enApiErrorCode _MapToApiErrorCode(enIdentityErrorCode identityError)
        {
            return identityError switch
            {
                enIdentityErrorCode.DefaultError => enApiErrorCode.UnknownError,
                enIdentityErrorCode.ConcurrencyFailure => enApiErrorCode.ConcurrencyError,
                enIdentityErrorCode.PasswordMismatch => enApiErrorCode.InvalidPassword,
                enIdentityErrorCode.InvalidToken => enApiErrorCode.InvalidToken,
                enIdentityErrorCode.RecoveryCodeRedemptionFailed => enApiErrorCode.InvalidRecoveryCode,
                enIdentityErrorCode.LoginAlreadyAssociated => enApiErrorCode.DuplicateLogin,
                enIdentityErrorCode.InvalidUserName => enApiErrorCode.InvalidUserName,
                enIdentityErrorCode.InvalidEmail => enApiErrorCode.InvalidEmail,
                enIdentityErrorCode.DuplicateUserName => enApiErrorCode.DuplicateUserName,
                enIdentityErrorCode.DuplicateEmail => enApiErrorCode.DuplicateEmail,
                enIdentityErrorCode.InvalidRoleName => enApiErrorCode.InvalidRoleName,
                enIdentityErrorCode.DuplicateRoleName => enApiErrorCode.DuplicateRoleName,
                enIdentityErrorCode.UserLockoutNotEnabled => enApiErrorCode.LockoutNotEnabled,
                enIdentityErrorCode.UserAlreadyInRole => enApiErrorCode.UserAlreadyInRole,
                enIdentityErrorCode.UserNotInRole => enApiErrorCode.UserNotInRole,
                enIdentityErrorCode.PasswordTooShort => enApiErrorCode.PasswordTooShort,
                enIdentityErrorCode.PasswordRequiresUniqueChars => enApiErrorCode.PasswordRequiresUniqueChars,
                enIdentityErrorCode.PasswordRequiresNonAlphanumeric => enApiErrorCode.PasswordRequiresNonAlphanumeric,
                enIdentityErrorCode.PasswordRequiresLower => enApiErrorCode.PasswordRequiresLower,
                enIdentityErrorCode.PasswordRequiresUpper => enApiErrorCode.PasswordRequiresUpper,
                enIdentityErrorCode.PasswordRequiresDigit => enApiErrorCode.PasswordRequiresDigit,
                enIdentityErrorCode.UserAlreadyHasPassword => enApiErrorCode.UserAlreadyHasPassword,

                 

                _ => enApiErrorCode.UnknownError  
            };
        }

    }
}
