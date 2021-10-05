using System;
using System.Linq;

namespace DPMGallery.Entities
{
    [Flags]
    public enum ApiKeyScope
    {
        PushNewPackage = 1,
        PushPackageVersion = 2,
        UnlistPackage = 4
    }
}
