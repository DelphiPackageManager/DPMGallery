using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DPMGallery.DTO
{
    public class PackageVersionsResponseDTO
    {
        /// <summary>
        /// The versions, lowercased and normalized.
        /// </summary>
        [JsonPropertyName("versions")]
        public IReadOnlyList<string> Versions { get; set; }
    }
}
