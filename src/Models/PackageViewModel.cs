using DPMGallery.Types;
using DPMGallery.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace DPMGallery.Models
{
    public class PackageViewModel : UISearchResultDTO
    {
        [JsonPropertyName("published")]
        public string Published { get; set; }
    }
}
