using System.Text.Json.Serialization;

namespace DPMGallery.DTO
{
    public class ServiceIndexItemDTO
    {
        /// <summary>
        /// The resource's base URL.
        /// </summary>
        [JsonPropertyName("@id")]
        public string ResourceUrl { get; set; }

        /// <summary>
        /// The resource's type.
        /// </summary>
        [JsonPropertyName("@type")]
        public string ResourceType { get; set; }

        /// <summary>
        /// Human readable comments about the resource.
        /// </summary>
        [JsonPropertyName("comment")]
        public string Comment { get; set; }
    }
}
