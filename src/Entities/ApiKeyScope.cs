using System;
using System.Linq;

namespace DPMGallery.Entities
{
    [Flags]
    public enum ApiKeyScope : uint
    {
        None = 0,
        PushNewPackage = 1 << 0, //1
        PushPackageVersion = 1 << 1, //2
        UnlistPackage = 1 << 2, //4
        All = ~(~0 << 3)
    }
}
