using DPMGallery.DTO;
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

            Mapping<UISearchResultDTO, PackageListItemModel>.Configure((dto, model) =>
            {
                model.PackageId = dto.PackageId;
                model.LatestVersion = dto.LatestVersion;
                model.IsPrerelease = dto.IsPreRelease;
                model.IsCommercial = dto.IsCommercial;
                model.IsTrial = dto.IsTrial;
                model.IsReservedPrefix = dto.IsReservedPrefix;
                model.Description = dto.Description;
                model.Owners = dto.Owners;
                model.Icon = dto.Icon;
                model.PublishedUtc = dto.PublishedUtc;
                model.Published = dto.PublishedUtc.ToPrettyDate();
                model.Tags = dto.Tags;
                model.TotalDownloads = dto.TotalDownloads;
                model.CompilerVersions = dto.CompilerVersions;
                model.Platforms = dto.Platforms;
            });

            Mapping<UISearchResponseDTO, PackagesListModel>.Configure((dto, model) =>
            {
                model.TotalPackages = dto.TotalHits;

                model.Packages = Mapping<UISearchResultDTO, PackageListItemModel>.Map(dto.Results);
            });

        }

    }
}
