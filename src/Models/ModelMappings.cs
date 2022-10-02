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

        }

    }
}
