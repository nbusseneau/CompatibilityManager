using CompatibilityManager.Enums;
using CompatibilityManager.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CompatibilityManager.Services
{
    public static class RegistryServices
    {
        #region AppCompatFlags registry keys

        private const string W8Prefix = "~";
        private const string AppCompatFlagsRegistryKey = @"Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers";

        private const string LastKeyRegistryKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Applets\Regedit";
        private const string LastKeyValueName = @"LastKey";

        private const string HKCUEnvVarRegistryKey = @"Environment";
        private const string HKLMEnvVarRegistryKey = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";

        public const string GlobalCompatEnvVarRegistryName = "__COMPAT_LAYER";

        private static string HKCU = $@"HKCU\{AppCompatFlagsRegistryKey}";
        private static string HKLM = $@"HKLM\{AppCompatFlagsRegistryKey}";

        public static RegistryKey HKCUKey { get; } = GetRegistryKey(hklm: false, global: false);
        public static RegistryKey HKLMKey { get; } = GetRegistryKey(hklm: true, global: false);
        public static RegistryKey HKCUEnvVarKey { get; } = GetRegistryKey(hklm: false, global: true);
        public static RegistryKey HKLMEnvVarKey { get; } = GetRegistryKey(hklm: true, global: true);

        #endregion

        private static RegistryKey GetRegistryKey(bool hklm = false, bool global = false)
        {
            try
            {
                if (global)
                {
                    if (hklm) { return Registry.LocalMachine.CreateSubKey(HKLMEnvVarRegistryKey); }
                    else { return Registry.CurrentUser.CreateSubKey(HKCUEnvVarRegistryKey); }
                }
                else
                {
                    if (hklm) { return Registry.LocalMachine.CreateSubKey(AppCompatFlagsRegistryKey); }
                    else { return Registry.CurrentUser.CreateSubKey(AppCompatFlagsRegistryKey); }
                }
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }

        /// <summary>
        /// Load applications and their associated settings from chosen registry key.
        /// </summary>
        public static IEnumerable<ApplicationViewModel> GetApplications(RegistryKey registryKey)
        {
            var applications = new List<ApplicationViewModel>();
            var regName = registryKey.ToString();

            if (regName.StartsWith("HKEY_CURRENT_USER") || regName.StartsWith("HKCU"))
            {
                applications.Add(new ApplicationViewModel(Resources.Strings.GlobalConfig, HKCUEnvVarKey));
            }
            else if (regName.StartsWith("HKEY_LOCAL_MACHINE") || regName.StartsWith("HKLM"))
            {
                applications.Add(new ApplicationViewModel(Resources.Strings.GlobalConfig, HKLMEnvVarKey));
            }

            foreach (var path in registryKey.GetValueNames())
            {
                applications.Add(new ApplicationViewModel(path, registryKey));
            }
            return applications;
        }

        /// <summary>
        /// Load a path (application) and its associated registry string (settings) from chosen registry key.
        /// </summary>
        public static string GetApplicationRegistryString(RegistryKey key, string path)
        {
            var str = path == Resources.Strings.GlobalConfig ? RegistryServices.GlobalCompatEnvVarRegistryName : path;
            return (string)key.GetValue(str);
        }

        /// <summary>
        /// Convert AppCompatFlag REG_SZ to their settings representation.
        /// </summary>
        public static Tuple<CompatibilityMode, ColorMode, DPIScaling, OtherFlags, List<string>> FromRegistryString(string registryString)
        {
            var substrings = new List<string>(registryString.Split());

            // On Windows 8 or above, a tilde is appended at beginning of the string.
            if (OSVersionServices.IsWindows8OrAbove && substrings.Contains(RegistryServices.W8Prefix))
            {
                substrings.Remove(RegistryServices.W8Prefix);
            }

            var compatibilityMode = CompatibilityModeServices.FromRegistryString(ref substrings);
            var colorMode = ColorModeServices.FromRegistryString(ref substrings);
            var dpiScaling = DPIScalingServices.FromRegistryString(ref substrings);
            var otherFlags = OtherFlagsServices.FromRegistryString(ref substrings);
            var additionalFlags = substrings; // Whatever wasn't matched by known flags
            
            return new Tuple<CompatibilityMode, ColorMode, DPIScaling, OtherFlags, List<string>>(compatibilityMode, colorMode, dpiScaling, otherFlags, additionalFlags);
        }

        /// <summary>
        /// Convert settings to their AppCompatFlag REG_SZ representation.
        /// </summary>
        public static string ToRegistryString(CompatibilityMode compatibilityMode, ColorMode colorMode, DPIScaling dpiScaling, OtherFlags otherFlags, List<string> additionalFlags)
        {
            var substrings = new List<string>();

            // Add all flags
            substrings.AddRange(new List<string>()
            {
                compatibilityMode.ToRegistryString(),
                colorMode.ToRegistryString(),
                dpiScaling.ToRegistryString(),
            });
            substrings.AddRange(otherFlags.ToRegistryString());
            substrings.AddRange(additionalFlags);
            substrings = substrings.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            // On Windows 8 or above, a tilde is appended at beginning of the string.
            if (substrings.Any() && OSVersionServices.IsWindows8OrAbove)
            {
                substrings.Insert(0, RegistryServices.W8Prefix);
            }

            var appCompatFlags = string.Join(" ", substrings);
            return appCompatFlags;
        }

        /// <summary>
        /// Open Regedit at AppCompatFlags registry location.
        /// </summary>
        public static void OpenRegedit(bool hklm = false)
        {
            foreach (var process in Process.GetProcessesByName("regedit")) { process.Kill(); }
            Registry.SetValue(LastKeyRegistryKey, LastKeyValueName, hklm ? HKLM : HKCU);
            Process.Start("regedit");
        }

        /// <summary>
        /// Import .reg file (requires elevated privileges).
        /// </summary>
        public static void ImportReg(string filename)
        {
            Process.Start("reg", $"import \"{filename}\"").WaitForExit();
        }

        /// <summary>
        /// Export .reg file of AppCompatFlags registry settings.
        /// </summary>
        public static void ExportReg(string filename, bool hklm = false)
        {
            Process.Start("reg", $"export \"{(hklm ? HKLM : HKCU)}\" \"{filename}\" /y");
        }
    }
}
