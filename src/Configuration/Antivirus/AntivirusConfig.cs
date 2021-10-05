using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Configuration
{
    public class AntivirusConfig
    {
        public ClamAVConfig ClamAVConfig { get; set; } = new();

        public VirusTotalConfig VirusTotalConfig { get; set; } = new();

    }
}
