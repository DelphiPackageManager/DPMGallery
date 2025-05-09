using System.Collections.Generic;

namespace DPMGallery.Models
{
    public class PublishedPackageModel
    {
        public string PackageId { get; set; }

        public string LatestVersion { get; set; }

        public IList<string> Owners { get; set; } = new List<string>();
    }
}
