using Dapper.Contrib.Extensions;
using DPMGallery.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace DPMGallery.Entities
{
    [Table(Constants.Database.TableNames.Users)]
    public class User
    {
        //
        // Summary:
        //     Gets or sets the primary key for this user.
        [PersonalData]
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("lockout_end")]
        public DateTimeOffset? LockoutEnd { get; set; }
        //
        // Summary:
        //     Gets or sets a flag indicating if two factor authentication is enabled for this
        //     user.
        [PersonalData]
        [Column("two_factor_enabled")]
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// The two-factor authenticator key.
        /// </summary>
        [Column("two_factor_authenticator_key")]
        public string TwoFactorAuthenticatorKey { get; set; }

        //
        // Summary:
        //     A random value that must change whenever a user is persisted to the store
        [Column("concurrency_stamp")]
        public string ConcurrencyStamp { get; set; }
        //
        // Summary:
        //     A random value that must change whenever a users credentials change (password
        //     changed, login removed)
        [Column("security_stamp")]
        public string SecurityStamp { get; set; }
        //
        // Summary:
        //     Gets or sets a salted and hashed representation of the password for this user.
        [Column("password_hash")]
        public string PasswordHash { get; set; }
        //
        // Summary:
        //     Gets or sets a flag indicating if a user has confirmed their email address.
        [PersonalData]
        [Column("email_confirmed")]
        public bool EmailConfirmed { get; set; }
        //
        // Summary:
        //     Gets or sets the normalized email address for this user.
        [Column("normalized_email")]
        public string NormalizedEmail { get; set; }
        //
        // Summary:
        //     Gets or sets the email address for this user.
        [ProtectedPersonalData]
        [Column("email")]
        public string Email { get; set; }
        //
        // Summary:
        //     Gets or sets the normalized user name for this user.
        [Column("normalized_user_name")]
        public string NormalizedUserName { get; set; }
        //
        // Summary:
        //     Gets or sets the user name for this user.
        [ProtectedPersonalData]
        [Column("user_name")]
        public string UserName { get; set; }

        //
        // Summary:
        //     Gets or sets a flag indicating if the user could be locked out.
        [Column("lockout_enabled")]
        public bool LockoutEnabled { get; set; }
        //
        // Summary:
        //     Gets or sets the number of failed login attempts for the current user.
        [Column("access_failed_count")]
        public int AccessFailedCount { get; set; }

        //
        // Summary:
        //     Gets or sets a flag indicating if a user has confirmed their telephone address.
        [PersonalData]
        [Column("phone_number_confirmed")]
        public bool PhoneNumberConfirmed { get; set; }
        //
        // Summary:
        //     Gets or sets a telephone number for the user.
        [ProtectedPersonalData]
        [Column("phone_number")]
        public string PhoneNumber { get; set; }


        [Column("is_organisation")]
        public bool IsOrganisation { get; set; }

        [Column("account_suspended")]
        public bool AccountSuspended { get; set; }

        //used by the identity stores
        internal List<Claim> Claims { get; set; }

        internal List<UserRole> Roles { get; set; }

        internal List<UserLoginInfo> Logins { get; set; }

        internal List<UserToken> Tokens { get; set; }

    }
}
