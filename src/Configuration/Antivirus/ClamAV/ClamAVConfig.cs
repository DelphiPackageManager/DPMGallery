using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Configuration
{
    public class ClamAVConfig
    {
        public string Host { get; set; } = "dpmgallery"; //just for testing on windows - wll be localhost when running on linux

        public int Port { get; set; } = 3310;

        public bool Enabled { get; set; } = true;
    }
}
