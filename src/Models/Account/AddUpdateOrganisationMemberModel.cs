using DPMGallery.Entities;

namespace DPMGallery.Models.Account
{
    public record AddUpdateOrganisationMemberModel(int orgId, string userName, MemberRole role);
    
}
