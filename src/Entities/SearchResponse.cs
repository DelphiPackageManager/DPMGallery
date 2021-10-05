using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Entities
{
    public class SearchResponse
    {
        public long TotalCount { get; set; }

        public IList<SearchResult> searchResults { get; set; }
    }
}
