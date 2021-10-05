using DPMGallery.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Entities
{
    public class SearchResult
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("versionid")]
        public int VersionId { get; set; }

        [Column("packageid")]
        public string PackageId { get; set; }

        [Column("compiler_version")]
        public CompilerVersion Compiler { get; set; }

        [Column("platform")]
        public Platform Platform { get; set; }

        [Column("latestversion")]
        public string LatestVersion { get; set; }

        [Column("lateststableversion")]
        public string LatestStableVersion { get; set; }

        [Column("is_prerelease")]
        public bool IsPreRelease { get; set; }

        [Column("is_commercial")]
        public bool IsCommercial { get; set; }

        [Column("is_trial")]
        public bool IsTrial { get; set; }

        [Column("is_reserved")]
        public bool IsReservedPrefix { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("authors")]
        public string Authors { get; set; }

        //not a field, we'll populate manually.
        public string Owners { get; set; }

        [Column("icon")]
        public string Icon { get; set; }

        [Column("read_me")]
        public string ReadMe { get; set; }

        [Column("release_notes")]
        public string ReleaseNotes { get; set; }

        [Column("filesize")]
        public long FileSize { get; set; }

        [Column("license")]
        public string License { get; set; }

        [Column("project_url")]
        public string ProjectUrl { get; set; }

        [Column("repository_url")]
        public string RepositoryUrl { get; set; }

        [Column("repository_type")]
        public string RepositoryType { get; set; }

        [Column("repository_branch")]
        public string RepositoryBranch { get; set; }

        [Column("repository_commit")]
        public string RepositoryCommit { get; set; }

        //not a db column - we'll fill it in later
        public string ReportUrl { get; set; }

        [Column("tags")]
        public string Tags { get; set; }

        [Column("total_downloads")]
        public long TotalDownloads { get; set; }

        [Column("version_downloads")]
        public long VersionDownloads { get; set; }

        public IList<PackageDependency> Dependencies { get; set; }

        [Column("listed")]
        public bool Listed { get; set; }

        [Column("deprecation_state")]
        public PackageDeprecationState DeprecationState { get; set; } = PackageDeprecationState.Ok;

        [Column("alternate_package")]
        public string AlternatePackage { get; set; }

        [Column("deprecation_message")]
        public string DeprecatedMessage { get; set; }

        [Column("status")]
        public PackageStatus Status { get; set; }

        [Column("published_utc")]
        public DateTime PublishedUtc { get; set; }

        [Column("hash")]
        public string Hash { get; set; }

        [Column("hash_algorithm")]
        public string HashAlgorithm { get; set; }

    }
}
