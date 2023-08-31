using Dapper.Contrib.Extensions;
using DPMGallery.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace DPMGallery.Entities
{
    [Table(Constants.Database.TableNames.Users)] //orgs are a special kind of user.
    public class Organisation
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_name")]
        public string Name { get; set; }

        //
        // Summary:
        //     Gets or sets the normalized user name for this user.
        [Column("normalized_user_name")]
        public string NormalizedUserName { get; set; }
        //

        [PersonalData]
        [Column ("email")]
        public string Email { get; set; }

        //
        // Summary:
        //     Gets or sets the normalized email address for this user.
        [Column("normalized_email")]
        public string NormalizedEmail { get; set; }
        //

        [PersonalData]
        [Column("email_confirmed")]
        public bool EmailConfirmed { get; set; }
    }
}
