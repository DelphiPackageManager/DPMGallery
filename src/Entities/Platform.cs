using System;
using System.Linq;

namespace DPMGallery.Entities
{

    public enum Platform
    {
        UnknownPlatform = 0,
        Win32 = 1,
        Win64 = 2,
        WinArm32 = 3 , //reserved for future use
        WinArm64 = 4, //reserved for future use
        OSX32 = 5,
        OSX64 = 6,
        OSXARM64 =7,
        AndroidArm32 = 8,
        AndroidArm64 = 9,
        AndroidIntel32 = 10, //reserved for future use
        AndroidIntel64 = 11, //reserved for future use
        Android = AndroidArm32, //for backwards compat with old packages
        Andriod64 = AndroidArm64,
        iOS32 = 12,
        iOS64 = 13, //reserved for future use
        LinuxIntel32 = 14, //reserved for future use
        LinuxIntel64 = 15,
        LinuxArm32 = 16, //reserved for future use
        LinuxArm64 = 17 //reserved for future use
    }

    public static class PlatformExtensions
    {
        public static Platform ToPlatform(this string value)
        {
            if (Enum.TryParse(value, true, out Platform platform))
                return platform;
            return Platform.UnknownPlatform;
        }
    }

}
