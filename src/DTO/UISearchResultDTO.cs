using DPMGallery.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace DPMGallery.DTO
{
    public class UISearchResultDTO
    {
        public string PackageId { get; set; }

        public string Version { get; set; }

        public string LatestVersion { get; set; }

        public string LatestStableVersion { get; set; }

        public bool IsPreRelease { get; set; }

        public bool IsCommercial { get; set; }

        public bool IsTrial { get; set; }

        public bool IsReservedPrefix { get; set; }

        public string Description { get; set; }

        public string Authors { get; set; }

        public List<string> Owners { get; set; }

        public string Icon { get; set; }

        public bool HasIcon => !string.IsNullOrEmpty(Icon);

        public string ReadMe { get; set; }

        public string ReleaseNotes { get; set; }

        public bool HasReadMe => !string.IsNullOrEmpty(ReadMe);

        public string License { get; set; }

        public string ProjectUrl { get; set; }

        public string RepositoryUrl { get; set; }

        public string RepositoryType { get; set; }

        public string RepositoryBranch { get; set; }

        public string RepositoryCommit { get; set; }

        public string ReportUrl { get; set; }

        public List<string> Tags { get; set; }

        public long TotalDownloads { get; set; }

        public bool Listed { get; set; }

        public PackageDeprecationState DeprecationState { get; set; } = PackageDeprecationState.Ok;

        public string AlternatePackage { get; set; }

        public string DeprecatedMessage { get; set; }

        public PackageStatus Status { get; set; }

        public DateTime PublishedUtc { get; set; }

        public string Hash { get; set; }

        public string HashAlgorithm { get; set; }

        public List<CompilerVersion> CompilerVersions { get; set; }

        public List<Platform> Platforms { get; set; }

    }
}

