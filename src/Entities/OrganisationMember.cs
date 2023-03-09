using Dapper.Contrib.Extensions;
using DPMGallery.Data;
using System;

namespace DPMGallery.Entities
{
    [Table(Constants.Database.TableNames.OrganisationMembers)]
    public class OrganisationMember
    {
        [Column("org_id")]
        public int OrgId { get; set; } //an organisation is a user with IsOrganisation set to true.

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("role")]
        public MemberRole Role { get; set; }

    }
}
