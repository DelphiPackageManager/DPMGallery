using Dapper.Contrib.Extensions;
using DPMGallery.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Entities
{
    [Table(Constants.Database.TableNames.PackageTargetPlatform)]
    public class PackageTargetPlatform
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("package_id")]
        public int PackageId { get; set; }

        [Column("compiler_version")]
        public CompilerVersion CompilerVersion { get; set; }

        [Column("platform")]
        public Platform Platform { get; set; }

        [Column("latest_version_id")]
        public int? LatestVersionId { get; set; }

        [Column("latest_stable_version_id")]
        public int? LatestStableVersionId { get; set; }

        public string SantisedCompilerVersion => $"{CompilerVersion}".Replace("RS", "").Replace('_', '.');

        public string FileName => $"{SantisedCompilerVersion}-{Platform}";
    }
}
