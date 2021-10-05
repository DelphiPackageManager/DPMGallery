using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Configuration
{
    public class VirusTotalConfig
    {
        public string APIEndPoint { get; set; }

        public string AccessKey { get; set; }

        public bool Enabled { get; set; } = false;
    }
}
