using CompatibilityManager.Enums;
using CompatibilityManager.ViewModels;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;

namespace CompatibilityManager.Services
{
    public static class RegistryServices
    {
        #region AppCompatFlags registry keys

        private static string AppCompatFlagsRegistryKey { get; } = @"Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers";

        public static RegistryKey HKCUKey { get; } = GetRegistryKey(hklm: false);
        public static RegistryKey HKLMKey { get; } = GetRegistryKey(hklm: true);

        #endregion

        private static RegistryKey GetRegistryKey(bool hklm = false)
        {
            if (hklm)
            {
                if (PrivilegesServices.IsRunAsAdmin) { return Registry.LocalMachine.OpenSubKey(AppCompatFlagsRegistryKey, true); }
                else { return null; }
            }
            else { return Registry.CurrentUser.OpenSubKey(AppCompatFlagsRegistryKey, true); }
        }

        public static IEnumerable<ApplicationViewModel> GetApplications(RegistryKey registryKey)
        {
            var applications = new List<ApplicationViewModel>();
            foreach (var path in registryKey.GetValueNames())
            {
                applications.Add(new ApplicationViewModel(path, registryKey));
            }
            return applications;
        }

        public static SettingsViewModel GetApplicationSettings(RegistryKey key, string path)
        {
            var registryString = (string)key.GetValue(path);
            if (!string.IsNullOrWhiteSpace(registryString)) { return new SettingsViewModel(registryString); }
            return new SettingsViewModel();
        }

        public static string ToRegistryString(CompatibilityMode compatibilityMode, ColorMode colorMode, DPIScaling dpiScaling, OtherFlags otherFlags)
        {
            var settings = new List<string>()
            {
                compatibilityMode.ToRegistryString(),
                colorMode.ToRegistryString(),
                dpiScaling.ToRegistryString(),
                otherFlags.ToRegistryString(),
            }.Where(s => !string.IsNullOrWhiteSpace(s));

            var appCompatFlags = string.Join(" ", settings);

            // On Windows 8 or above, a tilde is appended at beginning of the string.
            if (!string.IsNullOrWhiteSpace(appCompatFlags) && OSVersionServices.IsWindows8OrAbove)
            {
                appCompatFlags = string.Format("~ {0}", appCompatFlags);
            }

            return appCompatFlags;
        }
    }
}
