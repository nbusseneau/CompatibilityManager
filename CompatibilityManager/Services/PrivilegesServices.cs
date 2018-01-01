using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CompatibilityManager.Services
{
    public static class PrivilegesServices
    {
        public static BitmapSource ShieldIcon { get; } = StockIconServices.GetShieldBitmapSource();
        public static bool IsRunAsAdmin { get; } = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        public static void Elevate()
        {
            var adminProcess = new ProcessStartInfo
            {
                FileName = Assembly.GetExecutingAssembly().Location,
                Verb = "runas"
            };

            var uacPromptRefused = false;
            try { Process.Start(adminProcess); }
            catch (Win32Exception) { uacPromptRefused = true; } // Thrown if user answers "No" to the UAC prompt
            finally { if (!uacPromptRefused) { Application.Current.Shutdown(); } }
        }
    }
}
