using System.Threading.Tasks;
using System.Windows.Input;
using PasswordKeeper.Apps.Communication;
using PasswordKeeper.Apps.Wpf.Abstractions;
using PasswordKeeper.Apps.Wpf.Features.Accounts;
using PasswordKeeper.Apps.Wpf.Features.SecuritySettings;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.Requests;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.Responses;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.States;
using PasswordKeeper.Apps.Wpf.Messages.Switchers;
using PasswordKeeper.Apps.Wpf.Properties;
using PasswordKeeper.Services.Abstractions;
using PasswordKeeper.Services.CryptoProviders;

namespace PasswordKeeper.Apps.Wpf.Features.KeyChange
{
    public class KeyChangeViewModel : ValidatableViewModel, ISubscribable
    {
        private readonly IMessenger messenger;
        private readonly IUserService userService;

        private string password;

        private IStorageProvider storageProvider;

        public KeyChangeViewModel(IUserService userService, ICommandFactory commandFactory, IMessenger messenger)
        {
            this.userService = userService;
            this.messenger = messenger;

            SaveButtonClick = commandFactory.Create(OnSaveButtonClick);
            SecuritySettingsButtonClick = commandFactory.Create(OnSecuritySettingsButtonClick);
            BackButtonClick = commandFactory.Create(OnBackButtonClick);

            storageProvider = new DefaultStorageProvider();

            AddValidation();
        }

        #region View properties

        public string Password
        {
            get => password;

            set
            {
                password = value;
                Validate(nameof(Password));
            }
        }

        #endregion View properties

        #region Commands

        public ICommand SecuritySettingsButtonClick { get; }

        public ICommand SaveButtonClick { get; }

        public ICommand BackButtonClick { get; private set; }

        #endregion Commands

        #region Public methods

        public void Subscribe()
        {
            messenger.Subscribe<KeyChangeState>(this, OnStateChangeRequested);
            messenger.Subscribe<SetupSecurityResponseMessage<KeyChangeState>>(this, OnSetupSecurityResponseReceived);
        }

        public void Unsubscribe()
        {
            messenger.Unsubscribe<KeyChangeState>(this);
            messenger.Unsubscribe<SetupSecurityResponseMessage<KeyChangeState>>(this);
        }

        #endregion Public methods

        #region Event handlers

        private void OnSecuritySettingsButtonClick(object obj)
        {
            messenger.Send(
                this,
                new SwitchViewMessage
                {
                    NextView = typeof(SecuritySettingsViewModel)
                });

            messenger.Send(
                this,
                new SetupSecurityRequestMessage<KeyChangeState>
                {
                    ReturnToViewModel = typeof(KeyChangeViewModel),
                    State = new KeyChangeState
                    {
                        Password = Password
                    }
                });
        }

        private async void OnSaveButtonClick(object obj)
        {
            ValidateAll();
            if (HasErrors)
            {
                return;
            }

            messenger.Send(
                this,
                new SwitchLoadingIndicatorMessage
                {
                    Switcher = true
                });

            await Task.Run(() => userService.ChangeKey(storageProvider, Password)).ConfigureAwait(false);

            messenger.Send(
                this,
                new SwitchLoadingIndicatorMessage
                {
                    Switcher = false
                });

            messenger.Send(
                this,
                new SwitchViewMessage
                {
                    NextView = typeof(AccountsViewModel)
                });
        }

        private void OnStateChangeRequested(KeyChangeState state)
        {
            password = state.Password;
        }

        private void OnSetupSecurityResponseReceived(SetupSecurityResponseMessage<KeyChangeState> response)
        {
            OnStateChangeRequested(response.State);
            storageProvider = response.StorageProvider;
        }

        private void OnBackButtonClick(object obj)
        {
            messenger.Send(
                this,
                new SwitchViewMessage
                {
                    NextView = typeof(AccountsViewModel)
                });
        }

        #endregion Event handlers

        #region Private methods

        private void AddValidation()
        {
            AddRule(nameof(Password), () => !string.IsNullOrEmpty(Password), Errors.PasswordEmpty, true);
        }

        #endregion Private methods
    }
}