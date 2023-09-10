using DPMGallery.Data;

namespace DPMGallery.Entities
{
    public class OrganisationMember
    {
        [Column("org_id")]
        public int OrgId { get; set; } //an organisation is a user with IsOrganisation set to true.

        [Column("member_id")]
        public int MemberId { get; set; }

        [Column("user_name")]
        public string UserName { get; set; }

        [Column("member_role")]
        public MemberRole Role { get; set; }

        [Column("email")]
        public string Email { get; set; }
    }
}
