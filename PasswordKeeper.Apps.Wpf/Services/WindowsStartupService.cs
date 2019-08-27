using System.Windows.Forms;
using Microsoft.Win32;
using PasswordKeeper.Apps.Wpf.Abstractions.Services;

namespace PasswordKeeper.Apps.Wpf.Services
{
    public class WindowsStartupService : IStartupService
    {
        private const string RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string RegistryKey = "PasswordKeeper";

        public void Enable()
        {
            RegistryKey runOnStartupKey = Registry.CurrentUser.OpenSubKey(RegistryPath, true);
            if (runOnStartupKey.GetValue(RegistryKey) == null)
            {
                runOnStartupKey.SetValue(RegistryKey, Application.ExecutablePath, RegistryValueKind.String);
            }
        }

        public void Disable()
        {
            RegistryKey runOnStartupKey = Registry.CurrentUser.OpenSubKey(RegistryPath, true);
            if (runOnStartupKey.GetValue(RegistryKey) != null)
            {
                runOnStartupKey.DeleteValue(RegistryKey, false);
            }
        }
    }
}