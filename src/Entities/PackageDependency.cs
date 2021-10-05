using DPMGallery.Data;
using System;
using System.Linq;

namespace DPMGallery.Entities
{
    public class PackageDependency
    {
        [Column("packageversion_id")]
        public int PackageVersionId { get; set; }

        [Column("package_id")]
        public string PackageId { get; set; }
        [Column("version_range")]
        public string VersionRange { get; set; }

    }
}
