using Microsoft.AspNetCore.Identity;

namespace DPMGallery.Extensions
{
    public static class IdentityExtensions
    {
        public static bool IsValidationError(this IdentityErrorDescriber errorDescriber, string identityErrorCode)
        {
            switch (identityErrorCode)
            {

                //these are validation errors which are useful to the user
                case nameof(errorDescriber.PasswordMismatch):
                case nameof(errorDescriber.LoginAlreadyAssociated):
                case nameof(errorDescriber.InvalidUserName):
                case nameof(errorDescriber.InvalidEmail):
                case nameof(errorDescriber.DuplicateUserName):
                case nameof(errorDescriber.DuplicateEmail):
                case nameof(errorDescriber.PasswordTooShort):
                case nameof(errorDescriber.PasswordRequiresUniqueChars):
                case nameof(errorDescriber.PasswordRequiresNonAlphanumeric):
                case nameof(errorDescriber.PasswordRequiresDigit):
                case nameof(errorDescriber.PasswordRequiresLower):
                case nameof(errorDescriber.PasswordRequiresUpper):
                    return true;


                //these are not logic or system errors, and should logged but not shown to the user
                case nameof(errorDescriber.ConcurrencyFailure):
                case nameof(errorDescriber.InvalidToken):
                case nameof(errorDescriber.RecoveryCodeRedemptionFailed):
                case nameof(errorDescriber.UserAlreadyHasPassword):
                case nameof(errorDescriber.InvalidRoleName):
                case nameof(errorDescriber.DuplicateRoleName):
                case nameof(errorDescriber.UserLockoutNotEnabled):
                case nameof(errorDescriber.UserAlreadyInRole):
                case nameof(errorDescriber.UserNotInRole):
                default:
                    return false;
            }
        }

    }
}
