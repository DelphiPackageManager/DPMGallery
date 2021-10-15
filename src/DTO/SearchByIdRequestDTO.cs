using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DPMGallery.DTO
{
    public class SearchIdDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }
    }

    public class SearchByIdRequestDTO
    {
        [JsonPropertyName("packageids")]
        public List<SearchIdDTO> PackageIds { get; set; } = new();


        [JsonPropertyName("compiler")]       
        public string Compiler { get; set; }

        [JsonPropertyName("platform")]
        public string Platform { get; set; }
    }
}
