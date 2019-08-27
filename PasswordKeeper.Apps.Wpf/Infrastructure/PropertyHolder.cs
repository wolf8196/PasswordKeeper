using System;
using System.IO;
using PasswordKeeper.Apps.Wpf.Abstractions;
using PasswordKeeper.Services.Abstractions;

namespace PasswordKeeper.Apps.Wpf.Infrastructure
{
    public class PropertyHolder : IPropertyHolder
    {
        private readonly ISettings settings;

        public PropertyHolder(ISettings settings)
        {
            this.settings = settings;
        }

        public string UserDirectory
        {
            get
            {
                if (settings.IsFirstLaunch)
                {
                    settings.UserDirectoryLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    settings.IsFirstLaunch = false;
                }

                var userFolder = Path.Combine(settings.UserDirectoryLocation, settings.UserDirectoryName);

                if (!Directory.Exists(userFolder))
                {
                    Directory.CreateDirectory(userFolder);
                }

                return userFolder;
            }
        }
    }
}