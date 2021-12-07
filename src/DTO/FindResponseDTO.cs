using DPMGallery.Types;
using System.Text.Json.Serialization;

namespace DPMGallery.DTO
{
    public class FindResponseDTO
    {
        [JsonPropertyName("id")]
        public string PackageId { get; set; }

        [JsonPropertyName("compiler")]
        public CompilerVersion Compiler { get; set; }

        [JsonPropertyName("platform")]
        public Platform Platform { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

    }
}
