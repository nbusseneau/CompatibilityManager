using System;

namespace CompatibilityManager.Services
{
    public enum OSVersion
    {
        Windows_2000,
        Windows_XP,
        Windows_Vista,
        Windows_7,
        Windows_8,
        Windows_8_1,
        Windows_10,
    }

    public static class OSVersionServices
    {
        public static OSVersion? OSVersion { get; } = OSVersionServices.GetOSVersion();
        public static bool IsWindows8OrAbove { get; } = OSVersion >= Services.OSVersion.Windows_8;
        public static bool IsWindowsVistaOrAbove { get; } = OSVersion >= Services.OSVersion.Windows_Vista;

        private static OSVersion GetOSVersion()
        {
            var version = Environment.OSVersion.Version;

            if (version.Major == 10) { return Services.OSVersion.Windows_10; }
            else if (version.Major == 6)
            {
                if (version.Minor == 3) { return Services.OSVersion.Windows_8_1; }
                else if (version.Minor == 2) { return Services.OSVersion.Windows_8; }
                else if (version.Minor == 1) { return Services.OSVersion.Windows_7; }
                return Services.OSVersion.Windows_Vista;
            }
            if (version.Major == 5)
            {
                if (version.Minor >= 1) { return Services.OSVersion.Windows_XP; }
                return Services.OSVersion.Windows_2000;
            }
            
            throw new InvalidOperationException(string.Format(Resources.Strings.UnknownOSException, Resources.Strings.CatastrophicFailureException));
        }
    }
}
