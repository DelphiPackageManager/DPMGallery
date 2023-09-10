using DPMGallery.Data;
using DPMGallery.Models;
using System.Collections.Generic;

namespace DPMGallery.Entities
{
    public class UserOrganisation
    {
        public UserOrganisation()
        {
            Members = new List<OrganisationMember>();
        }

        [Column("org_id")]
        public int OrgId { get; set; }

        [Column("org_name")]
        public string OrganisationName { get; set; }

        [Column("email")]
        public string Email{ get; set; }

        [Column("member_id")]
        public int UserId { get; set; }

        [Column("member_role")]
        public MemberRole Role { get; set; }

        public int AdminCount { get; set; }
        public int CollaboratorCount { get; set; }

        public int PackageCount { get; set; }

        [Column("allow_contact")]
        public bool AllowContact { get; set; }

        [Column("notify_on_publish")]
        public bool NotifyOnPublish { get; set; }

        public List<OrganisationMember> Members { get; set; }

    }
}
