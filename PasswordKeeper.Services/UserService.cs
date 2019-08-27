using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using PasswordKeeper.Services.Abstractions;
using PasswordKeeper.Services.Entities;
using PasswordKeeper.Services.Exceptions;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Services
{
    public class UserService : IUserService
    {
        private const string UserKey = @"/\user";

        private readonly IDatabaseService databaseService;
        private readonly IUserManager userManager;
        private readonly ISerializer<User> serializer;

        public UserService(IDatabaseService databaseService, IUserManager userManager, ISerializer<User> serializer)
        {
            this.databaseService = databaseService.ThrowIfNull(nameof(databaseService));
            this.userManager = userManager.ThrowIfNull(nameof(userManager));
            this.serializer = serializer.ThrowIfNull(nameof(serializer));
        }

        public User User { get; private set; }

        public async Task CreateAsync(IStorageProvider storageProvider, string userName, string password)
        {
            storageProvider.ThrowIfNull(nameof(storageProvider));
            userName.ThrowIfNullOrEmpty(nameof(userName));
            password.ThrowIfNullOrEmpty(nameof(password));

            if (userManager.UserExists(userName))
            {
                throw new UserExistsException(userName);
            }

            userManager.MarkUserLoggedIn(userName);

            await databaseService.SetupAsync(
                userManager.GetUserLocation(userName),
                storageProvider.GetStorage(),
                Encoding.UTF8.GetBytes(password)).ConfigureAwait(false);

            var user = new User();

            var userData = SerializeUser(user);
            await databaseService.CurrentDatabase.AddAsync(UserKey, userData).ConfigureAwait(false);
            await databaseService.CurrentDatabase.SaveAsync().ConfigureAwait(false);

            User = user;
        }

        public async Task LogInAsync(string userName, string password)
        {
            userName.ThrowIfNullOrEmpty(nameof(userName));
            password.ThrowIfNullOrEmpty(nameof(password));

            if (!userManager.UserExists(userName))
            {
                throw new UserNotFoundException(userName);
            }

            if (userManager.IsUserLoggedIn(userName))
            {
                throw new UserLoggedException(userName);
            }

            userManager.MarkUserLoggedIn(userName);

            var userData = new MemoryStream();

            try
            {
                if (!await databaseService
                    .OpenAsync(userManager.GetUserLocation(userName), Encoding.UTF8.GetBytes(password))
                    .ConfigureAwait(false))
                {
                    throw new AuthenticationException();
                }

                await databaseService.CurrentDatabase.GetAsync(UserKey, userData).ConfigureAwait(false);
            }
            catch
            {
                databaseService.Close();
                userManager.MarkUserLoggedOut(userName);
                throw new AuthenticationException();
            }

            userData.Seek(0, SeekOrigin.Begin);
            User = serializer.Deserialize(userData);
        }

        public Task LogOutAsync()
        {
            if (User != null)
            {
                User = null;
                userManager.MarkUserLoggedOut(
                    userManager.GetUserNameByLocation(
                        databaseService.CurrentDatabase.FileName));
                databaseService.Close();
            }

            return Task.CompletedTask;
        }

        public async Task BeginSaveAsync()
        {
            CheckUser();
            await databaseService.CurrentDatabase.RemoveAsync(UserKey).ConfigureAwait(false);
        }

        public async Task EndSaveAsync()
        {
            CheckUser();

            var userData = SerializeUser(User);

            await databaseService.CurrentDatabase.AddAsync(UserKey, userData).ConfigureAwait(false);

            await databaseService.CurrentDatabase.SaveAsync().ConfigureAwait(false);
        }

        public async Task SaveAsync()
        {
            CheckUser();

            var userData = SerializeUser(User);

            await databaseService.CurrentDatabase.RemoveAsync(UserKey).ConfigureAwait(false);

            await databaseService.CurrentDatabase.AddAsync(UserKey, userData).ConfigureAwait(false);

            await databaseService.CurrentDatabase.SaveAsync().ConfigureAwait(false);
        }

        public async Task ChangeKey(IStorageProvider storageProvider, string password)
        {
            storageProvider.ThrowIfNull(nameof(storageProvider));
            password.ThrowIfNullOrEmpty(nameof(password));
            CheckUser();

            await databaseService.CurrentDatabase.ResetupAsync(storageProvider.GetStorage(), Encoding.UTF8.GetBytes(password)).ConfigureAwait(false);
        }

        private void CheckUser()
        {
            if (User == null)
            {
                throw new InvalidOperationException("No user is logged in. Call CreateAsync or LogInAsync first");
            }
        }

        private MemoryStream SerializeUser(User user)
        {
            var stream = new MemoryStream();
            serializer.Serialize(stream, user);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }
}