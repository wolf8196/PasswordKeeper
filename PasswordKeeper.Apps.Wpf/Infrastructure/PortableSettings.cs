using PasswordKeeper.Apps.Wpf.Abstractions;

namespace PasswordKeeper.Apps.Wpf.Infrastructure
{
    public class PortableSettings : ISettings
    {
        public bool IsFirstLaunch
        {
            get => false; set { }
        }

        public string UserDirectoryName
        {
            get => "Users"; set { }
        }

        public string UserDirectoryLocation
        {
            get => string.Empty; set { }
        }
    }
}