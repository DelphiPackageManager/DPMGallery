using DPMGallery.Entities;
using DPMGallery.Utils;
using System.Text.Json.Serialization;

namespace DPMGallery.Models
{
    public class OrganisationMemberModel
    {
        public int OrgId { get; set; } //an organisation is a user with IsOrganisation set to true.
        public int MemberId { get; set; }
        public string UserName { get; set; }

        [JsonConverter(typeof(SerializePropertyAsDefaultConverter<MemberRole>))]
        public MemberRole Role { get; set; }
        public string AvatarUrl { get; set; }
    }
}
