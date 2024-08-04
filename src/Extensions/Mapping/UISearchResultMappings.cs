using DPMGallery.Entities;
using DPMGallery.Models;
using DPMGallery.Utils;

using System.Collections.Generic;
using System.Linq;

namespace DPMGallery.Extensions.Mapping
{
    public static class UISearchResultMappings
    {
        public static PackageListItemModel ToModel(this UISearchResult entity)
        {
            return new PackageListItemModel()
            {
                PackageId = entity.PackageId,
                Version = entity.Version,
                LatestVersion = entity.LatestVersion,
                IsPrerelease = entity.IsPreRelease,
                IsCommercial = entity.IsCommercial,
                IsTrial = entity.IsTrial,
                IsReservedPrefix = entity.IsReservedPrefix,
                Description = entity.Description,
                Owners = entity.Owners,
                OwnerInfos = entity.OwnerInfos.ToModels(),
                Icon = entity.Icon,
                PublishedUtc = entity.PublishedUtc,
                Published = entity.PublishedUtc.ToPrettyDate(),
                //some older packages have comma separated tags
                Tags = string.IsNullOrEmpty(entity.Tags) ? null : entity.Tags.Replace(',', ' ').Split(' ').Select(x => x.Trim().ToLower()).ToList(),
                TotalDownloads = entity.TotalDownloads,
                CompilerVersions = entity.CompilerVersions,
                Platforms = entity.Platforms

            };
        }

        public static List<PackageListItemModel> ToModel(this IList<UISearchResult> entities)
        {
            if (entities == null)
                return null;

            var items = new List<PackageListItemModel>();
            foreach (UISearchResult entity in entities)
                items.Add(entity.ToModel());
            return items;
        }
    }
}
