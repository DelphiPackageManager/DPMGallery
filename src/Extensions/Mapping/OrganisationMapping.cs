using DPMGallery.Data;
using DPMGallery.Entities;
using DPMGallery.Models;
using DPMGallery.Models.Account;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace DPMGallery.Extensions.Mapping
{
    public static class OrganisationMapping
    {
        public static UserOrganisationModel ToModel(this UserOrganisation entity)
        {
            //var model = new ApiKeyModel(entity.Id, entity.Name, entity.Key, entity.UserId, entity.ExpiresUTC, entity.GlobPattern, entity.Packages, entity.Scopes, entity.Revoked);
            var hash = entity.Email.ToLower().ToMd5();

            var model = new UserOrganisationModel()
            {
                Id = entity.OrgId,
                UserId = entity.UserId,
                Name = entity.OrganisationName,
                AdminCount = entity.AdminCount,
                CollaboratorCount = entity.CollaboratorCount,
                Email = entity.Email,
                AllowContact = entity.AllowContact,
                NotifyOnPublish = entity.NotifyOnPublish,
                PackageCount = entity.PackageCount,
                Role = entity.Role.ToString(),
                AvatarUrl = $"https://www.gravatar.com/avatar/{hash}",
                Members = entity.Members.Select(x => x.ToModel()).ToList()
            };

            return model;
        }

        public static OrganisationMemberModel ToModel(this OrganisationMember entity)
        {
            var hash = entity.Email.ToLower().ToMd5();

            var memberModel = new OrganisationMemberModel()
            {
                OrgId = entity.OrgId,
                MemberId = entity.MemberId,
                Role = entity.Role,
                UserName = entity.UserName,
                AvatarUrl = $"https://www.gravatar.com/avatar/{hash}"
            };
            return memberModel;
        }

        public static List<UserOrganisationModel> ToModels(this IList<UserOrganisation> entities)
        {
            if (entities == null)
                return null;

            var items = new List<UserOrganisationModel>();
            foreach (UserOrganisation entity in entities)
                items.Add(entity.ToModel());
            return items;
        }

        public static PagedList<UserOrganisationModel> ToPagedModel(this PagedList<UserOrganisation> entities)
        {
            if (entities == null)
                return null;

            List<UserOrganisationModel> items = entities.Items.ToModels();

            if (items == null)
                return null;

            var pagedList = new PagedList<UserOrganisationModel>(items, entities.TotalCount, entities.Paging);

            return pagedList;


        }
    }
}
