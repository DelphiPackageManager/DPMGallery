using DPMGallery.Types;
using System;
using System.Collections.Generic;

namespace DPMGallery.Models
{

    public class PackageDetailsModel
    {
        public PackageDetailsModel()
        {
            Versions = new List<PackageVersionModel>();
            CompilerPlatforms = new List<PackageDetailCompilerModel>();
            Licenses = new List<string>();
            Tags = new List<string>();
            Owners = new List<PackageOwnerModel>();
        }
        
        public string PackageId { get; set; }

        public string PackageName { get; set; }

        public string PackageVersion { get; set; }

        public string Description { get; set; }

        public bool PrefixReserved { get; set; }

        public bool IsPrerelease { get; set; }

        public bool IsLatestVersion { get; set; }

        public bool IsCommercial { get; set; }

        public bool IsTrial { get; set; }

        public string Icon { get; set; }

        public DateTime PublishedUtc { get; set; }

        public string Published { get; set; } //prettydate

        public List<PackageVersionModel> Versions { get; set; }

        public List<PackageDetailCompilerModel> CompilerPlatforms { get; set; }

        public string ProjectUrl { get; set; } 

        public string ReadMe { get; set; }
        public string RepositoryUrl { get; set; }

        public List<string> Licenses { get; set; }

        public List<PackageOwnerModel> Owners { get; set; }

        public long TotalDownloads { get; set; }

        public long CurrentVersionDownload { get; set; } = 0;

        public List<string> Tags { get; set; }

    }
}
