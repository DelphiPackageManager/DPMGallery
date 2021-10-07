using System.Collections.Generic;
using DPMGallery.Types;

namespace DPMGallery.Entities
{
    public class UISearchResult : SearchResult
    {
        public List<CompilerVersion> CompilerVersions { get; set; }

        public List<Platform> Platforms { get; set; }

    }
}
