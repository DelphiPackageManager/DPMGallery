using DPMGallery.Entities;

namespace DPMGallery.Models
{
    public class UserOrganisationModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public MemberRole Role { get; set; }
        public int MemberCount { get; set; }
        public int PackageCount { get; set;}
    }
}
