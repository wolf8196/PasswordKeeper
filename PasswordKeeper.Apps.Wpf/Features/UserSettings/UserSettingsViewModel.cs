using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using PasswordKeeper.Apps.Communication;
using PasswordKeeper.Apps.Wpf.Abstractions;
using PasswordKeeper.Apps.Wpf.Abstractions.Services;
using PasswordKeeper.Apps.Wpf.Features.Accounts;
using PasswordKeeper.Apps.Wpf.Messages.Switchers;
using PasswordKeeper.Services.Abstractions;
using PasswordKeeper.Services.Entities;

namespace PasswordKeeper.Apps.Wpf.Features.UserSettings
{
    public class UserSettingsViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly IMessenger messenger;
        private readonly IStartupService startupService;
        private readonly IUserService userService;

        private Settings settings;

        public UserSettingsViewModel(
            IUserService userService,
            ICommandFactory commandFactory,
            IMessenger messenger,
            IStartupService startupService)
        {
            this.userService = userService;
            this.messenger = messenger;
            this.startupService = startupService;

            SaveButtonClick = commandFactory.Create(OnSaveButtonClick);
            BackButtonClick = commandFactory.Create(OnBackButtonClick);
            Settings = new Settings(User.Settings);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region View properties

        public Settings Settings
        {
            get
            {
                return settings;
            }

            set
            {
                settings = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Settings)));
            }
        }

        #endregion View properties

        #region Commands

        public ICommand BackButtonClick { get; }

        public ICommand SaveButtonClick { get; }

        #endregion Commands

        #region Private properties

        private User User { get => userService.User; }

        #endregion Private properties

        #region Event handlers

        private void OnBackButtonClick(object obj)
        {
            messenger.Send(
                this,
                new SwitchViewMessage
                {
                    NextView = typeof(AccountsViewModel)
                });
        }

        private async void OnSaveButtonClick(object obj)
        {
            messenger.Send(
                this,
                new SwitchLoadingIndicatorMessage
                {
                    Switcher = true
                });

            User.Settings = Settings;

            messenger.Send(
                this,
                new SwitchOffClosing
                {
                    Switcher = true
                });

            await Task.Run(() => userService.SaveAsync()).ConfigureAwait(false);

            if (Settings.StartWithWindows)
            {
                startupService.Enable();
            }
            else
            {
                startupService.Disable();
            }

            messenger.Send(
                this,
                new SwitchOffClosing
                {
                    Switcher = false
                });

            messenger.Send(
                this,
                new SwitchViewMessage
                {
                    NextView = typeof(AccountsViewModel)
                });

            messenger.Send(
                this,
                new SwitchLoadingIndicatorMessage
                {
                    Switcher = false
                });
        }

        #endregion Event handlers
    }
}