using DPMGallery.Data;

namespace DPMGallery.Entities
{
    public class UserOrganisation
    {
        [Column("org_id")]
        public int OrgId { get; set; }

        [Column("org_name")]
        public string OrganisationName { get; set; }

        [Column("member_id")]
        public int UserId { get; set; }

        [Column("role")]
        public MemberRole Role { get; set; }

        public int MemberCount { get; set; }

        public int PackageCount { get; set; }

    }
}
