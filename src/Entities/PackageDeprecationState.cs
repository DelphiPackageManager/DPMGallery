using System;
using System.Linq;

namespace DPMGallery.Entities
{
    [Flags]
    public enum PackageDeprecationState
    {
        Ok = 0,
        Legacy = 1,
        CriticalBugs = 2,
        Other = 3
    }
}
