using RealEstate.Infrastructure.Identity.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Infrastructure.Identity.IdentiyErrors.Enums;
 
public enum enIdentityErrorCode
{
    [ErrorCategory(enErrorCategory.Default)]
    [ErrorDescription("An unknown error occurred")]
    [HttpStatusCode(enHttpStatusCode.InternalServerError)]
    DefaultError,

    [ErrorCategory(enErrorCategory.Concurrency)]
    [ErrorDescription("Optimistic concurrency failure, object has been modified")]
    [HttpStatusCode(enHttpStatusCode.InternalServerError)]
    ConcurrencyFailure,

    [ErrorCategory(enErrorCategory.Password)]
    [ErrorDescription("Incorrect password")]
    [HttpStatusCode(enHttpStatusCode.Unauthorized)]
    PasswordMismatch,

    [ErrorCategory(enErrorCategory.Token)]
    [ErrorDescription("Invalid token")]
    [HttpStatusCode(enHttpStatusCode.Unauthorized)]
    InvalidToken,

    [ErrorCategory(enErrorCategory.Token)]
    [ErrorDescription("Recovery code is invalid or has already been used")]
    [HttpStatusCode(enHttpStatusCode.BadRequest)]
    RecoveryCodeRedemptionFailed,

    [ErrorCategory(enErrorCategory.Authentication)]
    [ErrorDescription("A user with this login already exists")]
    [HttpStatusCode(enHttpStatusCode.Conflict)]
    LoginAlreadyAssociated,

    [ErrorCategory(enErrorCategory.Username)]
    [ErrorDescription("Username is invalid")]
    [HttpStatusCode(enHttpStatusCode.BadRequest)]
    InvalidUserName,

    [ErrorCategory(enErrorCategory.Email)]
    [ErrorDescription("Email is invalid")]
    [HttpStatusCode(enHttpStatusCode.BadRequest)]
    InvalidEmail,

    [ErrorCategory(enErrorCategory.Username)]
    [ErrorDescription("Username already exists")]
    [HttpStatusCode(enHttpStatusCode.Conflict)]
    DuplicateUserName,

    [ErrorCategory(enErrorCategory.Email)]
    [ErrorDescription("Email already in use")]
    [HttpStatusCode(enHttpStatusCode.Conflict)]
    DuplicateEmail,

    [ErrorCategory(enErrorCategory.Role)]
    [ErrorDescription("Role name is invalid")]
    [HttpStatusCode(enHttpStatusCode.BadRequest)]
    InvalidRoleName,

    [ErrorCategory(enErrorCategory.Role)]
    [ErrorDescription("Role name already exists")]
    [HttpStatusCode(enHttpStatusCode.Conflict)]
    DuplicateRoleName,



    [ErrorCategory(enErrorCategory.User)]
    [ErrorDescription("Lockout is not enabled for this user")]
    [HttpStatusCode(enHttpStatusCode.Forbidden)]
    UserLockoutNotEnabled,

    [ErrorCategory(enErrorCategory.Role)]
    [ErrorDescription("User already in this role")]
    [HttpStatusCode(enHttpStatusCode.Conflict)]
    UserAlreadyInRole,

    [ErrorCategory(enErrorCategory.Role)]
    [ErrorDescription("User is not in this role")]
    [HttpStatusCode(enHttpStatusCode.NotFound)]
    UserNotInRole,

    [ErrorCategory(enErrorCategory.Password)]
    [ErrorDescription("Password is too short")]
    [HttpStatusCode(enHttpStatusCode.BadRequest)]
    PasswordTooShort,

    [ErrorCategory(enErrorCategory.Password)]
    [ErrorDescription("Password requires more unique characters")]
    [HttpStatusCode(enHttpStatusCode.BadRequest)]
    PasswordRequiresUniqueChars,

    [ErrorCategory(enErrorCategory.Password)]
    [ErrorDescription("Password must contain at least one special character")]
    [HttpStatusCode(enHttpStatusCode.BadRequest)]
    PasswordRequiresNonAlphanumeric,

    [ErrorCategory(enErrorCategory.Password)]
    [ErrorDescription("Password requires at least one lowercase letter")]
    [HttpStatusCode(enHttpStatusCode.BadRequest)]
    PasswordRequiresLower,

    [ErrorCategory(enErrorCategory.Password)]
    [ErrorDescription("Password requires at least one uppercase letter")]
    [HttpStatusCode(enHttpStatusCode.BadRequest)]
    PasswordRequiresUpper,

    [ErrorCategory(enErrorCategory.Password)]
    [ErrorDescription("Password must contain at least one digit")]
    [HttpStatusCode(enHttpStatusCode.BadRequest)]
    PasswordRequiresDigit,

    [ErrorCategory(enErrorCategory.Password)]
    [ErrorDescription("User already has a password set")]
    [HttpStatusCode(enHttpStatusCode.Conflict)]
    UserAlreadyHasPassword,


}