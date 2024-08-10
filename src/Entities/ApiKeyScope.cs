using System;
using System.Linq;

namespace DPMGallery.Entities
{
    [Flags]
    public enum ApiKeyScope : uint
    {
        None = 0,
        PushPackageVersion = 1, //011
        PushNewPackage = 3, //011 - includes push new version
        UnlistPackage = 4, //100
        All = 7 //111
    }
}
