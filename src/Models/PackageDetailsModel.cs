using DPMGallery.Types;
using System;
using System.Collections.Generic;

namespace DPMGallery.Models
{
    public class PackageDetailsModel
    {
        public string PackageId { get; set; }

        public string PackageName { get; set; }

        public string PackageVersion { get; set; }

        public bool PrefixReserved { get; set; }

        public bool IsPrerelease { get; set; }

        public bool IsLatestVersion { get; set; }

        public bool IsCommercial { get; set; }

        public bool IsTrial { get; set; }

        public DateTime PublishedUtc { get; set; }

        public string Published { get; set; } //prettydate

        public List<Platform> Platforms { get; set; }

        public List<CompilerVersion> CompilerVersions { get; set; }

        public List<PackageVersionModel> Versions { get; set; }

        public string ProjectUrl { get; set; } 

        public string ReadMe { get; set; }
        public string RepositoryUrl { get; set; }

        public List<string> Licenses { get; set; }

        public List<string> Owners { get; set; }

        public long TotalDownloads { get; set; }

        public int CurrentVersionDownload { get; set; } = 0;

        public List<string> Tags { get; set; }

        public Dictionary<CompilerVersion, List<Platform>> CompilerPlatforms { get; set; }

        public Dictionary<CompilerVersion, Dictionary<Platform, List<PackageDependencyModel>>> PackageDependencies { get; set; }
       
    }
}
