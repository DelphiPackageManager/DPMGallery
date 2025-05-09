using System.Collections.Generic;

namespace DPMGallery.Entities
{
    public class UIPublishedPackageItem
    {
        public string PackageId { get; set; }

        public string LatestVersion { get; set; }

        public long Downloads { get; set; }

        public IList<string> Owners { get; set; } = new List<string>();
    }
}
