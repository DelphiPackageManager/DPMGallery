using DPMGallery.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DPMGallery.Models
{
    public class EditableOrganisationModel
    {
        public int OrgId { get; set; }

        public string OrganisationName { get; set; }

        public string Email { get; set; }

        public bool CanContact { get; set; }

        public bool NotifyOnPublish { get; set; }

        public IList<OrganisationMemberModel> Members { get; set; }
    }
}
