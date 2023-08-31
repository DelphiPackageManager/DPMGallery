using DPMGallery.Entities;

namespace DPMGallery.Models
{
    public class OrganisationMemberModel
    {
        public int OrgId { get; set; } //an organisation is a user with IsOrganisation set to true.
        public int UserId { get; set; }
        public string UserName { get; set; }
        public MemberRole Role { get; set; }

    }
}
