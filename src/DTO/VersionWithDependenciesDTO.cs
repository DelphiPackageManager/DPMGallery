using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DPMGallery.DTO
{
    public class VersionWithDependenciesDTO
    {
        public string Version { get; set; }

        public IReadOnlyList<DependencyDTO> Dependencies { get; set; }
    }
}
