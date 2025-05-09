using System.Collections.Generic;

namespace DPMGallery.Entities
{
    public class UIPublishedPackageResponse
    {
        public long TotalCount { get; set; }

        public long TotalDownloads { get; set; }
        public IList<UIPublishedPackageItem> searchResults { get; set; } = new List<UIPublishedPackageItem>();
    }
}
