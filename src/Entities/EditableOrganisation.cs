using Dapper.Contrib.Extensions;
using DPMGallery.Data;
using Microsoft.AspNetCore.Identity;
using System.Collections;
using System.Collections.Generic;

namespace DPMGallery.Entities
{
    public class EditableOrganisation
    {
        [Column("org_id")]
        public int OrgId { get; set; }

        [Column("org_name")]
        public string OrganisationName { get; set; }

        [PersonalData]
        [Column("email")]
        public string Email { get; set; }

        public bool CanContact { get; set; }

        public bool NotifyOnPublish { get; set; }

        public IEnumerable<OrganisationMember> Members { get; set; }

    }
}
