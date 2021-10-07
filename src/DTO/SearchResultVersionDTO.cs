using DPMGallery.Types;
using System.Text.Json.Serialization;

namespace DPMGallery.DTO
{
    public class SearchResultVersionDTO
    {
        /// <summary>
        /// The package's full dpm version after normalization, including any SemVer 2.0.0 build metadata.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The downloads for this single version of the matched package.
        /// </summary>
        public long Downloads { get; set; }

        /// <summary>
        /// The compiler versions supported in this version.
        /// </summary>
        public CompilerVersion CompilerVersion { get; set; }

        /// <summary>
        /// The platforms supported in this verison.
        /// </summary>
        [JsonPropertyName("platform")]
        public Platform Platform { get; set; }
    }
}
