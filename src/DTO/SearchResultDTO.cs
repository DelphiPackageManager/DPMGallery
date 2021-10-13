using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using DPMGallery.Types;

namespace DPMGallery.DTO
{
    /// <summary>
    /// A package search result
    /// </summary>
    public class SearchResultDTO
    {
        [JsonPropertyName("id")]
        public string PackageId { get; set; }

        [JsonPropertyName("compiler")]
        public string Compiler { get; set; }

        [JsonPropertyName("platform")]
        public Platform Platform { get; set; }

        [JsonPropertyName("latestVersion")]
        public string LatestVersion { get; set; }

        [JsonPropertyName("latestStableVersion")]
        public string LatestStableVersion { get; set; }

        [JsonPropertyName("isPreRelease")]
        public bool IsPreRelease { get; set; }

        [JsonPropertyName("isCommercial")]
        public bool IsCommercial { get; set; }

        [JsonPropertyName("isTrial")]
        public bool IsTrial { get; set; }

        [JsonPropertyName("isReservedPrefix")] 
        public bool IsReservedPrefix { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("authors")]
        public string Authors { get; set; }

        [JsonPropertyName("owners")]
        public List<string> Owners { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("hasIcon")]
        public bool HasIcon => !string.IsNullOrEmpty(Icon);

        [JsonPropertyName("readMe")]
        public string ReadMe { get; set; }

        [JsonPropertyName("releaseNotes")]
        public string ReleaseNotes { get; set; }

        [JsonPropertyName("hasReadMe")]
        public bool HasReadMe => !string.IsNullOrEmpty(ReadMe);

        [JsonPropertyName("license")]
        public string License { get; set; }

        [JsonPropertyName("projectUrl")]
        public string ProjectUrl { get; set; }

        [JsonPropertyName("repositoryUrl")]
        public string RepositoryUrl { get; set; }

        [JsonPropertyName("repositoryType")]
        public string RepositoryType { get; set; }

        [JsonPropertyName("repositoryBranch")]
        public string RepositoryBranch { get; set; }

        [JsonPropertyName("repositoryCommit")]
        public string RepositoryCommit { get; set; }

        [JsonPropertyName("reportUrl")]
        public string ReportUrl { get; set; }

        [JsonPropertyName("tags")]
        public string Tags { get; set; }

        [JsonPropertyName("searchPaths")]
        public string SearchPaths { get; set; }

        [JsonPropertyName("totalDownloads")]
        public long TotalDownloads { get; set; }

        [JsonPropertyName("listed")]
        public bool Listed { get; set; }

        [JsonPropertyName("deprecationState")]
        public PackageDeprecationState DeprecationState { get; set; } = PackageDeprecationState.Ok;

        [JsonPropertyName("alternatePackage")]
        public string AlternatePackage { get; set; }

        [JsonPropertyName("deprecationMessage")]
        public string DeprecatedMessage { get; set; }

        [JsonPropertyName("status")]
        public PackageStatus Status { get; set; }

        [JsonPropertyName("publishedUtc")]
        public DateTime PublishedUtc { get; set; }

        [JsonPropertyName("hash")]
        public string Hash { get; set; }

        [JsonPropertyName("hashAlgorith")]
        public string HashAlgorithm { get; set; }

        [JsonPropertyName("dependencies")]
        public IReadOnlyList<DependencyDTO> Dependencies { get; set; }
    }
}
