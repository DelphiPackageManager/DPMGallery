using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DPMGallery.DTO
{
    public class VersionWithDependenciesDTO
    {
        [JsonPropertyName("id")]
        public string PackageId { get; set; }

        [JsonPropertyName("compiler")]
        public string Compiler { get; set; }

        [JsonPropertyName("platform")]
        public string Platform { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("hash")]
        public string Hash {  get; set; }

        [JsonPropertyName("hashAlgorithm")]
        public string HashAlgorithm { get; set; }

        [JsonPropertyName("dependencies")]
        public IReadOnlyList<DependencyDTO> Dependencies { get; set; }
    }
}
