using System;

namespace DPMGallery.Models
{
    public class PackageVersionModel
    {
        public string Version { get; set; }

        public long Downloads { get; set; }

        public string Published { get; set; }

        public DateTime PublishedUtc { get; set; }

    }
}
