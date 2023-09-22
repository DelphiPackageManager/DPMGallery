using System;
using System.Linq;

namespace DPMGallery.Entities
{
    [Flags]
    public enum ApiKeyScope : uint
    {
        PushNewPackage = 1,
        PushPackageVersion = 2,
        UnlistPackage = 4
    }
}
