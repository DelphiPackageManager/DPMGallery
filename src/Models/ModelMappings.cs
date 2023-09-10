using DPMGallery.DTO;
using DPMGallery.Entities;
using DPMGallery.Extensions;
using DPMGallery.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Models
{
    public static class ModelMappings
    {
        public static void Configure()
        {

            Mapping<UISearchResult, PackageListItemModel>.Configure((r, model) =>
            {
                model.PackageId = r.PackageId;
                model.Version = r.Version;
                model.LatestVersion = r.LatestVersion;
                model.IsPrerelease = r.IsPreRelease;
                model.IsCommercial = r.IsCommercial;
                model.IsTrial = r.IsTrial;
                model.IsReservedPrefix = r.IsReservedPrefix;
                model.Description = r.Description;
                model.Owners = r.Owners;
                model.Icon = r.Icon;
                model.PublishedUtc = r.PublishedUtc;
                model.Published = r.PublishedUtc.ToPrettyDate();
                //some older packages have comma separated tags
                model.Tags = string.IsNullOrEmpty(r.Tags) ? null : r.Tags.Replace(',', ' ').Split(' ').Select(x => x.Trim().ToLower()).ToList();
                model.TotalDownloads = r.TotalDownloads;
                model.CompilerVersions = r.CompilerVersions;
                model.Platforms = r.Platforms;
            });

            Mapping<UISearchResponse, PackagesListModel>.Configure((r, model) =>
            {
                model.TotalPackages = r.TotalCount;

                model.Packages = Mapping<UISearchResult, PackageListItemModel>.Map(r.searchResults);
            });

            Mapping<PackageDependency, PackageDependencyModel>.Configure((entity, model) =>
            {
                model.PackageId = entity.PackageId;
                model.VersionRange = entity.VersionRange;
            });


            Mapping<ApiKey, ApiKeyModel>.Configure((entity, model) =>
            {
                model.Id = entity.Id;
                model.UserId = entity.UserId;
                model.Name = entity.Name;
                model.Packages = entity.Packages;
                model.ExpiresUTC = entity.ExpiresUTC;
                model.GlobPattern = entity.GlobPattern;
                model.Scopes = (int)entity.Scopes;
                model.Revoked = entity.Revoked;
                //never send hashed key
            });

            Mapping<ApiKeyModel, ApiKey>.Configure((model, entity) =>
            {
                entity.Id = model.Id;
                entity.UserId = model.UserId;
                entity.Name = model.Name;
                entity.Packages = model.Packages;
                entity.ExpiresUTC = model.ExpiresUTC;
                entity.GlobPattern = model.GlobPattern;
                entity.Scopes = (ApiKeyScope)model.Scopes;
                entity.Revoked = model.Revoked;
                entity.Key = model.Key;
            });

            Mapping<UserOrganisation, UserOrganisationModel>.Configure((entity, model) =>
            {
                model.Id = entity.OrgId;
                model.UserId = entity.UserId;
                model.Name = entity.OrganisationName;
                model.Email = entity.Email;
                model.Role = (int)entity.Role;
                model.PackageCount = entity.PackageCount;
                model.CollaboratorCount = entity.CollaboratorCount;
                model.AdminCount = entity.AdminCount;
                model.AllowContact = entity.AllowContact;
                model.NotifyOnPublish = entity.NotifyOnPublish;

                foreach (var member in entity.Members)
                {
                    var memberModel = Mapping<OrganisationMember, OrganisationMemberModel>.Map(member);
                    model.Members.Add(memberModel);
                }
                
            });


            Mapping<UserOrganisationModel, UserOrganisation>.Configure((model, entity) =>
            {
                entity.OrgId = model.Id;
                entity.UserId = model.UserId;
                entity.OrganisationName = model.Name;
                entity.Email = model.Email;
                entity.Role = (MemberRole)model.Role;
                entity.PackageCount = model.PackageCount;
                entity.CollaboratorCount = model.CollaboratorCount;
                entity.AdminCount = model.AdminCount;
                entity.AllowContact = model.AllowContact;
                entity.NotifyOnPublish = model.NotifyOnPublish;
                foreach (var memberModel in model.Members)
                {
                    var member = Mapping<OrganisationMemberModel, OrganisationMember>.Map(memberModel);
                    entity.Members.Add(member);
                }

            });


            Mapping<OrganisationMember, OrganisationMemberModel>.Configure((entity, model) => 
            {
                model.OrgId = entity.OrgId;
                model.MemberId = entity.MemberId;
                model.UserName = entity.UserName;
                model.Role = (int)entity.Role;
                var hash = entity.Email.ToLower().ToMd5();
                model.AvatarUrl = $"https://www.gravatar.com/avatar/{hash}";
            });


            Mapping<OrganisationMemberModel, OrganisationMember>.Configure((model, entity) =>
            {
                entity.OrgId = model.OrgId;
                entity.MemberId = model.MemberId;
                entity.UserName = model.UserName;
                entity.Role = (MemberRole)model.Role;
            });


            Mapping<EditableOrganisation, EditableOrganisationModel>.Configure((entity, model) =>
            {
                model.OrgId = entity.OrgId;
                model.OrganisationName = entity.OrganisationName;
                model.Email = entity.Email;
                model.CanContact = entity.CanContact;
                model.NotifyOnPublish = entity.NotifyOnPublish;
                model.Members = Mapping<OrganisationMember, OrganisationMemberModel>.Map(entity.Members);
            });
        }
    }
}
