using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Entities
{
    public class ApiSearchResponse
    {
        public long TotalCount { get; set; }

        public IList<SearchResult> searchResults { get; set; }
    }
}
