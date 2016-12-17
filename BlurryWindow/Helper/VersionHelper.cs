using System;
using System.Linq;
using System.Management;
using OperatingSystem = BlurryControls.Internals.OperatingSystem;

namespace BlurryControls.Helper
{
    [Obsolete("Was meant to determine the current system version")]
    public class VersionHelper
    {
        private static OperatingSystem _operatingSystem = OperatingSystem.NotSupported;

        public static OperatingSystem OperatingSystem
        {
            get
            {
                if (_operatingSystem == OperatingSystem.NotSupported) _operatingSystem = GetOperatorSystem();
                return _operatingSystem;
            }
        }

        private static OperatingSystem GetOperatorSystem()
        {
            var win32OperatorSystemCaption = Win32_OperationSystem_Caption();
            return win32OperatorSystemCaption.Contains("Microsoft Windows 10")
                ? OperatingSystem.Windows10
                : (win32OperatorSystemCaption.Contains("Microsoft Windows 7")
                || win32OperatorSystemCaption.Contains("Microsoft Windows 8")
                    ? OperatingSystem.Windows78
                    : OperatingSystem.NotSupported);
        }

        private static string Win32_OperationSystem_Caption()
        {
            var name =
                (from x in
                    new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get()
                        .Cast<ManagementObject>()
                 select x.GetPropertyValue("Caption")).FirstOrDefault();
            return name?.ToString() ?? string.Empty;
        }
    }
}