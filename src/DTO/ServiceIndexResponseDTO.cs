using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace DPMGallery.DTO
{
    public class ServiceIndexResponseDTO
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("resources")]
        public IReadOnlyList<ServiceIndexItemDTO> Resources { get; set; }
    }
}
