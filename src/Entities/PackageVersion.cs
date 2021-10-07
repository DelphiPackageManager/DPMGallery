using DPMGallery.Types;
using Dapper.Contrib.Extensions;
using DPMGallery.Data;
using System;
using System.Collections.Generic;

namespace DPMGallery.Entities
{
    [Table(Constants.Database.TableNames.PackageVersion)]
    public class PackageVersion
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("targetplatform_id")]
        public int TargetPlatformId { get; set; }

        [Column("version")]
        public string Version { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("copyright")]
        public string Copyright { get; set; }

        [Column("is_prerelease")]
        public bool IsPrerelease { get; set; }

        [Column("is_commercial")]
        public bool IsCommercial { get; set; }

        [Column("is_trial")]
        public bool IsTrial { get; set; }


        [Column("authors")]
        public string Authors { get; set; }

        [Column("icon")]
        public string Icon { get; set; }

        public bool HasIcon => !String.IsNullOrEmpty(this.Icon);

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

        [Column("read_me")]
        public string ReadMe { get; set; }
        public bool HasReadMe => !String.IsNullOrEmpty(this.ReadMe);

        [Column("release_notes")]
        public string ReleaseNotes { get; set; }

        [Column("filesize")]
        public long FileSize { get; set; }

        [Column("listed")]
        public bool Listed { get; set; }

        [Column("published_utc")]
        public DateTime PublishedUtc { get; set; }

        [Column("downloads")]
        public long Downloads { get; set; }

        [Column("deprecation_state")]
        public PackageDeprecationState DeprecationState { get; set; } = PackageDeprecationState.Ok;

        [Column("alternate_package")]
        public string AlternatePackage { get; set; }

        [Column("deprecation_message")]
        public string DeprecatedMessage { get; set; }

        [Column("status")]
        public PackageStatus Status { get; set; }

        [Column("status_message")]
        public string StatusMessage { get; set; }

        [Column("tags")]
        public string Tags { get; set; }

        [Column("hash")]
        public string Hash { get; set; }

        [Column("hash_algorithm")]
        public string HashAlgorithm { get; set; }

        public PackageVersion()
        {
            DeprecationState = PackageDeprecationState.Ok;
            Status = PackageStatus.Queued;
            Listed = true;
            Dependencies = new List<PackageDependency>();
        }

        public SemanticVersioning.Version SemVer => SemanticVersioning.Version.Parse(Version);

        public IList<PackageDependency> Dependencies { get; set; }
    }
}
