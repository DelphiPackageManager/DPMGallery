using System;
using System.Collections.Generic;
using DPMGallery.Data;

namespace DPMGallery.Statistics
{
    public class PackageDownloads
    {
        [Column("packageid")]
        public string PackageId { get; set; }
        [Column("downloads")]
        public int Downloads { get; set; }
    }

    public class VersionDownloads : PackageDownloads 
    {
        [Column("version")]
        public string Version { get; set; }
    }

    public class StatisticsData
    {
        private readonly List<PackageDownloads> _packageDownloads;
        private readonly List<VersionDownloads> _versionDownloads;
        public StatisticsData() 
        { 
            _packageDownloads = new List<PackageDownloads>();
            _versionDownloads = new List<VersionDownloads>();
            LastUpdated = DateTime.UtcNow;
        }

        public int TotalDownloads { get; set; }

        public int UniquePackages { get; set; }

        public int PackageVersions { get; set; }    

        public DateTime LastUpdated { get; set; }

        public List<PackageDownloads> TopPackageDownloads => _packageDownloads;

        public List<VersionDownloads> TopVersionDownloads => _versionDownloads;

        public static StatisticsData Instance { get;  } = new StatisticsData();
    }
}
