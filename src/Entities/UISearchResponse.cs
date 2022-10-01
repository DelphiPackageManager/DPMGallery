using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Entities
{
    public class UISearchResponse
    {
        public long TotalCount { get; set; }

        public long TotalDownloads { get; set; }

        public IList<UISearchResult> searchResults { get; set; } = new List<UISearchResult>();

    }
}
