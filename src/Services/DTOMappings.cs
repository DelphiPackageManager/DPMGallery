using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DPMGallery.Utils;
using DPMGallery.DTO;
using DPMGallery.Entities;

namespace DPMGallery.Services
{
    public static class DTOMappings
    {
        public static void Configure()
        {
            Mapping<PackageDependency, DependencyDTO>.Configure((d, dto) =>
            {
                dto.PackageId = d.PackageId;
                dto.VersionRange = d.VersionRange;
            });

            Mapping<PackageVersion, VersionWithDependenciesDTO>.Configure((v, dto) =>
            {
                dto.Version = v.Version;
                dto.Dependencies = Mapping<PackageDependency, DependencyDTO>.Map(v.Dependencies).ToList();
            });


            Mapping<SearchResult, SearchResultDTO>.Configure((r, dto) =>
            {
                dto.PackageId = r.PackageId;
                dto.Compiler = r.Compiler;
                dto.Platform = r.Platform;
                dto.LatestVersion = r.LatestVersion;
                dto.LatestStableVersion = r.LatestStableVersion;
                dto.IsPreRelease = r.IsPreRelease;
                dto.IsCommercial = r.IsCommercial;
                dto.IsTrial = r.IsTrial;
                dto.IsReservedPrefix = r.IsReservedPrefix;
                dto.Description = r.Description;
                dto.Authors = r.Authors;
                dto.Owners = r.Owners;
                dto.Icon = r.Icon;
                dto.ReadMe = r.ReadMe;
                dto.ReleaseNotes = r.ReleaseNotes;
                dto.License = r.License;
                dto.ProjectUrl = r.ProjectUrl;
                dto.RepositoryUrl = r.RepositoryUrl;
                dto.RepositoryType = r.RepositoryType;
                dto.RepositoryBranch = r.RepositoryBranch;
                dto.RepositoryCommit = r.RepositoryCommit;
                dto.ReportUrl = r.ReportUrl;
                dto.Status = r.Status;
                dto.PublishedUtc = r.PublishedUtc;
                dto.DeprecatedMessage = r.DeprecatedMessage;
                dto.DeprecationState = r.DeprecationState;
                dto.AlternatePackage = r.AlternatePackage;
                dto.Tags = r.Tags;
                dto.Hash = r.Hash;
                dto.HashAlgorithm = r.HashAlgorithm;
                dto.TotalDownloads = r.TotalDownloads;
                if (r.Dependencies != null)
                    dto.Dependencies = Mapping<PackageDependency, DependencyDTO>.Map(r.Dependencies).ToList();
            });

            Mapping<ApiSearchResponse, SearchResponseDTO>.Configure((r, dto) =>
            {
                dto.TotalHits = r.TotalCount;

                dto.Results = Mapping<SearchResult, SearchResultDTO>.Map(r.searchResults);
            });


            Mapping<UISearchResult, UISearchResultDTO>.Configure((r, dto) =>
            {
                dto.PackageId = r.PackageId;
                dto.LatestVersion = r.LatestVersion;
                dto.LatestStableVersion = r.LatestStableVersion;
                dto.IsPreRelease = r.IsPreRelease;
                dto.IsCommercial = r.IsCommercial;
                dto.IsTrial = r.IsTrial;
                dto.IsReservedPrefix = r.IsReservedPrefix;
                dto.Description = r.Description;
                dto.Authors = r.Authors;
                dto.Owners = r.Owners;
                dto.Icon = r.Icon;
                dto.ReadMe = r.ReadMe;
                dto.ReleaseNotes = r.ReleaseNotes;
                dto.License = r.License;
                dto.ProjectUrl = r.ProjectUrl;
                dto.RepositoryUrl = r.RepositoryUrl;
                dto.RepositoryType = r.RepositoryType;
                dto.RepositoryBranch = r.RepositoryBranch;
                dto.RepositoryCommit = r.RepositoryCommit;
                dto.ReportUrl = r.ReportUrl;
                dto.Status = r.Status;
                dto.PublishedUtc = r.PublishedUtc;
                dto.DeprecatedMessage = r.DeprecatedMessage;
                dto.DeprecationState = r.DeprecationState;
                dto.AlternatePackage = r.AlternatePackage;
                //some older packages have comma separated tags
                dto.Tags = string.IsNullOrEmpty(r.Tags) ? null : r.Tags.Replace(',',' ').Split(' ').Select(x => x.Trim().ToLower()).ToList();
                dto.Hash = r.Hash;
                dto.HashAlgorithm = r.HashAlgorithm;
                dto.TotalDownloads = r.TotalDownloads;
                dto.CompilerVersions = r.CompilerVersions;
                dto.Platforms = r.Platforms;
            });

            Mapping<UISearchResponse, UISearchResponseDTO>.Configure((r, dto) =>
            {
                dto.TotalHits = r.TotalCount;

                dto.Results = Mapping<UISearchResult, UISearchResultDTO>.Map(r.searchResults);
            });
        }
    }
}
