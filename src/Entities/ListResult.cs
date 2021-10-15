using DPMGallery.Data;
using DPMGallery.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Entities
{
    public class ListResult
    {
        [Column("packageid")]
        public string PackageId { get; set; }

        [Column("compiler_version")]
        public CompilerVersion Compiler { get; set; }

        [Column("platform")]
        public Platform Platform { get; set; }

        [Column("version")]
        public string Version { get; set; }
    }
}
