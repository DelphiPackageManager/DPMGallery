using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Entities
{
    public class ApiSearchResponse
    {
        public long TotalCount { get; set; }

        public List<SearchResult> searchResults { get; set; } = new();
    }
}
