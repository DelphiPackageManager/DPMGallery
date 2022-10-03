using DPMGallery.Types;
using System.Collections.Generic;
using System.Security.Policy;

namespace DPMGallery.Models
{
    public class PackageDetailPlatformModel
    {
        public PackageDetailPlatformModel(Platform platform)
        {
            Platform = platform;
            Dependencies = new List<PackageDependencyModel>();
        }

        public Platform Platform { get; set; }

        public List<PackageDependencyModel> Dependencies { get; set; }

        public string DownloadUrl { get; set; }
    }
}
