﻿using DPMGallery.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DPMGallery.DTO
{
    public class ListResultDTO
    {
        [JsonPropertyName("id")]
        public string PackageId { get; set; }

        [JsonPropertyName("compiler")]
        public string Compiler { get; set; }

        [JsonPropertyName("platforms")]
        public string Platforms { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }
    }
}
