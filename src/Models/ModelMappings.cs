using DPMGallery.DTO;
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

            Mapping<UISearchResultDTO, PackageViewModel>.Configure((dto, model) =>
            {
                model.PackageId = dto.PackageId;
                model.Version = dto.Version;
                model.LatestVersion = dto.LatestVersion;
                model.LatestStableVersion = dto.LatestStableVersion;
                model.IsPreRelease = dto.IsPreRelease;
                model.IsCommercial = dto.IsCommercial;
                model.IsTrial = dto.IsTrial;
                model.IsReservedPrefix = dto.IsReservedPrefix;
                model.Description = dto.Description;
                model.Authors = dto.Authors;
                model.Owners = dto.Owners;
                model.Icon = dto.Icon;
                model.ReadMe = dto.ReadMe;
                model.ReleaseNotes = dto.ReleaseNotes;
                model.License = dto.License;
                model.ProjectUrl = dto.ProjectUrl;
                model.RepositoryUrl = dto.RepositoryUrl;
                model.RepositoryType = dto.RepositoryType;
                model.RepositoryBranch = dto.RepositoryBranch;
                model.RepositoryCommit = dto.RepositoryCommit;
                model.ReportUrl = dto.ReportUrl;
                model.Status = dto.Status;
                model.PublishedUtc = dto.PublishedUtc;
                model.DeprecatedMessage = dto.DeprecatedMessage;
                model.DeprecationState = dto.DeprecationState;
                model.AlternatePackage = dto.AlternatePackage;
                model.Tags = dto.Tags;
                model.Hash = dto.Hash;
                model.HashAlgorithm = dto.HashAlgorithm;
                model.TotalDownloads = dto.TotalDownloads;
                model.CompilerVersions = dto.CompilerVersions;
                model.Platforms = dto.Platforms;
            });

            Mapping<UISearchResponseDTO, PackagesViewModel>.Configure((dto, model) =>
            {
                model.TotalPackages = dto.TotalHits;

                model.Packages = Mapping<UISearchResultDTO, PackageViewModel>.Map(dto.Results);
            });

        }

    }
}
