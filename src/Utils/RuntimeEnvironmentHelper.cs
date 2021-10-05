using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DPMGallery.Utils
{
    public static class RuntimeEnvironmentHelper
    {
       
        private static readonly Lazy<bool> _isWindows = new Lazy<bool>(() => GetIsWindows());

        private static readonly Lazy<bool> _IsMacOSX = new Lazy<bool>(() => GetIsMacOSX());

        private static readonly Lazy<bool> _IsLinux = new Lazy<bool>(() => GetIsLinux());

        [DllImport("libc")]
        static extern int uname(IntPtr buf);

        public static bool IsWindows
        {
            get => _isWindows.Value;
        }

        private static bool GetIsWindows()
        {
#if IS_CORECLR
            // This API does work on full framework but it requires a newer nuget client (RID aware)
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                return true;
            }

            return false;
#else
            var platform = (int)Environment.OSVersion.Platform;
            return (platform != 4) && (platform != 6) && (platform != 128);
#endif
        }

        //private static string GetCurrentProcessFilePath()
        //{
        //    using (var process = Process.GetCurrentProcess())
        //    {
        //        return process.MainModule.FileName;
        //    }
        //}

        public static bool IsMacOSX
        {
            get => _IsMacOSX.Value;
        }

        private static bool GetIsMacOSX()
        {
#if IS_CORECLR
            // This API does work on full framework but it requires a newer nuget client (RID aware)
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
            {
                return true;
            }

            return false;
#else
            var buf = IntPtr.Zero;

            try
            {
                buf = Marshal.AllocHGlobal(8192);

                // This is a hacktastic way of getting sysname from uname ()
                if (uname(buf) == 0)
                {
                    var os = Marshal.PtrToStringAnsi(buf);

                    if (os == "Darwin")
                    {
                        return true;
                    }
                }
            }
            catch
            {
            }
            finally
            {
                if (buf != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buf);
                }
            }

            return false;
#endif
        }

        public static bool IsLinux
        {
            get => _IsLinux.Value;
        }

        private static bool GetIsLinux()
        {
#if IS_CORECLR
            // This API does work on full framework but it requires a newer nuget client (RID aware)
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                return true;
            }

            return false;
#else
            var platform = (int)Environment.OSVersion.Platform;
            return platform == 4;
#endif
        }
    }
}
