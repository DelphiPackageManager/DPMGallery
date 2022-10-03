using DPMGallery.Types;
using System.Collections.Generic;

namespace DPMGallery.Models
{
    public class PackageDetailCompilerModel
    {
        public PackageDetailCompilerModel(CompilerVersion compilerVersion)
        {
            CompilerVersion = compilerVersion;
            Platforms = new List<PackageDetailPlatformModel>();
        }

        public CompilerVersion CompilerVersion { get; set; }

        public List<PackageDetailPlatformModel> Platforms { get; private set; }
    }
}
