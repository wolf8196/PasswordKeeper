using PasswordKeeper.Apps.Wpf.Abstractions;

namespace PasswordKeeper.Apps.Wpf.Infrastructure
{
    public class DesktopSettings : ISettings
    {
        public bool IsFirstLaunch
        {
            get
            {
                return Properties.Settings.Default.IsFirstLaunch;
            }

            set
            {
                Properties.Settings.Default.IsFirstLaunch = value;
                Properties.Settings.Default.Save();
            }
        }

        public string UserDirectoryName
        {
            get
            {
                return Properties.Settings.Default.UserDirectoryName;
            }

            set
            {
                Properties.Settings.Default.UserDirectoryName = value;
                Properties.Settings.Default.Save();
            }
        }

        public string UserDirectoryLocation
        {
            get
            {
                return Properties.Settings.Default.UserDirectoryLocation;
            }

            set
            {
                Properties.Settings.Default.UserDirectoryLocation = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}