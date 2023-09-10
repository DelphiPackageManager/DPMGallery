using DPMGallery.Entities;
using System.Collections;
using System.Collections.Generic;

namespace DPMGallery.Models
{
    public class UserOrganisationModel
    {
        public UserOrganisationModel()
        {
            Members = new List<OrganisationMemberModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }
        public int UserId { get; set; }
        public int Role { get; set; }
        public int AdminCount { get; set; }
        public int CollaboratorCount { get; set; }
        public int PackageCount { get; set; }

        public bool AllowContact { get; set;}

        public bool NotifyOnPublish { get; set;}

        public List<OrganisationMemberModel> Members { get; set; }
    }
}
