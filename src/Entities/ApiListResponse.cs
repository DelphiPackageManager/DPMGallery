using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Entities
{
    public class ApiListResponse
    {
        public long TotalCount { get; set; }

        public IList<ListResult> searchResults { get; set; }
    }
}
