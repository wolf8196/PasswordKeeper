using System.IO;
using PasswordKeeper.Services.Abstractions;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Services
{
    public class UserManager : IUserManager
    {
        private readonly IPropertyHolder properties;
        private readonly ILocker locker;

        public UserManager(IPropertyHolder properties, ILocker locker)
        {
            this.properties = properties.ThrowIfNull(nameof(properties));
            this.locker = locker.ThrowIfNull(nameof(locker));
        }

        public bool UserExists(string userName)
        {
            userName.ThrowIfNullOrEmpty(nameof(userName));

            return File.Exists(
                Path.Combine(properties.UserDirectory, AddExtension(userName)));
        }

        public bool IsUserLoggedIn(string userName)
        {
            userName.ThrowIfNullOrEmpty(nameof(userName));

            return locker.IsLocked(
                Path.Combine(properties.UserDirectory, AddExtension(userName)));
        }

        public void MarkUserLoggedIn(string userName)
        {
            userName.ThrowIfNullOrEmpty(nameof(userName));

            locker.Lock(GetUserLocation(userName));
        }

        public void MarkUserLoggedOut(string userName)
        {
            userName.ThrowIfNullOrEmpty(nameof(userName));

            locker.Unlock(GetUserLocation(userName));
        }

        public string GetUserLocation(string userName)
        {
            userName.ThrowIfNullOrEmpty(nameof(userName));

            return Path.Combine(properties.UserDirectory, AddExtension(userName));
        }

        public string GetUserNameByLocation(string location)
        {
            location.ThrowIfNullOrEmpty(nameof(location));

            return Path.GetFileNameWithoutExtension(location);
        }

        private static string AddExtension(string fileName)
        {
            return $"{fileName}.pkdb";
        }
    }
}