using Dapper.Contrib.Extensions;
using DPMGallery.Data;
using Microsoft.AspNetCore.Identity;
using System;

namespace DPMGallery.Entities
{
    public class OrganisationMember
    {
        [Column("org_id")]
        public int OrgId { get; set; } //an organisation is a user with IsOrganisation set to true.

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("user_name")]
        public string UserName { get; set; }

        [Column("role")]
        public MemberRole Role { get; set; }

    }
}
