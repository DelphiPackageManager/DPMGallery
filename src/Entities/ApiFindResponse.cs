using DPMGallery.Data;
using DPMGallery.Types;

namespace DPMGallery.Entities
{
    public class ApiFindResponse
    {
        [Column("packageid")]
        public string PackageId { get; set; }

        [Column("compiler_version")]
        public CompilerVersion Compiler { get; set; }

        [Column("platform")]
        public Platform Platform { get; set; }

        [Column("version")]
        public string Version { get; set; }

        [Column("hash")]
        public string Hash { get; set; }

        [Column("hashAlgorithm")]
        public string HashAlgorithm { get; set; }
    }
}
