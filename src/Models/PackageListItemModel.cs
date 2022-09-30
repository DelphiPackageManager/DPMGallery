using DPMGallery.Types;
using DPMGallery.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace DPMGallery.Models
{
    public class PackageListItemModel 
    {
        public string PackageId { get; set; }

        public string LatestVersion { get; set; }

        public bool IsPrerelease { get; set; }

        public bool IsCommercial { get; set; }

        public bool IsTrial { get; set; }

        public bool IsReservedPrefix { get; set; }

        public string Description { get; set; }

        public List<string> Owners { get; set; }

        public string Icon { get; set; }

        public bool HasIcon => !string.IsNullOrEmpty(Icon);

        public List<string> Tags { get; set; }

        public long TotalDownloads { get; set; }

        public DateTime PublishedUtc { get; set; }

        public List<CompilerVersion> CompilerVersions { get; set; }

        public List<Platform> Platforms { get; set; }

        [JsonPropertyName("published")]
        public string Published { get; set; }
    }
}
