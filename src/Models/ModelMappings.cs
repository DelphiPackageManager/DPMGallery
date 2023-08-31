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
                model.Role = entity.Role;
            });

            Mapping<OrganisationMember, OrganisationMemberModel>.Configure((entity, model) => 
            {
                model.OrgId = entity.OrgId;
                model.UserId = entity.UserId;
                model.UserName = entity.UserName;
                model.Role = entity.Role;
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
