using DPMGallery.Entities;

namespace DPMGallery.Models
{
    public class OrganisationMemberModel
    {
        public int OrgId { get; set; } //an organisation is a user with IsOrganisation set to true.
        public int MemberId { get; set; }
        public string UserName { get; set; }
        public int Role { get; set; }
        public string AvatarUrl { get; set; }
    }
}
