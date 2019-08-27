using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using PasswordKeeper.Apps.Communication;
using PasswordKeeper.Apps.Wpf.Abstractions;
using PasswordKeeper.Apps.Wpf.Features.Accounts;
using PasswordKeeper.Apps.Wpf.Features.Authentication;
using PasswordKeeper.Apps.Wpf.Features.SecuritySettings;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.Requests;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.Responses;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.States;
using PasswordKeeper.Apps.Wpf.Messages.Switchers;
using PasswordKeeper.Apps.Wpf.Properties;
using PasswordKeeper.Services.Abstractions;
using PasswordKeeper.Services.CryptoProviders;
using PasswordKeeper.Services.Exceptions;

namespace PasswordKeeper.Apps.Wpf.Features.Registration
{
    public class RegistrationViewModel : ValidatableViewModel, ISubscribable
    {
        private readonly IMessenger messenger;
        private readonly IUserService userService;

        private string login;
        private string password;

        private IStorageProvider storageProvider;

        public RegistrationViewModel(IUserService userService, ICommandFactory commandFactory, IMessenger messenger)
        {
            this.userService = userService;
            this.messenger = messenger;

            SaveButtonClick = commandFactory.Create(OnSaveButtonClick);
            AuthButtonClick = commandFactory.Create(OnAuthButtonClick);
            SecuritySettingsButtonClick = commandFactory.Create(OnSecuritySettingsButtonClick);

            storageProvider = new DefaultStorageProvider();

            AddValidation();
        }

        #region View properties

        public string Login
        {
            get => login;

            set
            {
                login = value;
                Validate(nameof(Login));
            }
        }

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

        public ICommand AuthButtonClick { get; }

        public ICommand SecuritySettingsButtonClick { get; }

        public ICommand SaveButtonClick { get; }

        #endregion Commands

        #region Public methods

        public void Subscribe()
        {
            messenger.Subscribe<RegistrationState>(this, OnStateChangeRequested);
            messenger.Subscribe<SetupSecurityResponseMessage<RegistrationState>>(this, OnSetupSecurityResponseReceived);
        }

        public void Unsubscribe()
        {
            messenger.Unsubscribe<RegistrationState>(this);
            messenger.Unsubscribe<SetupSecurityResponseMessage<RegistrationState>>(this);
        }

        #endregion Public methods

        #region Event handlers

        private void OnAuthButtonClick(object obj)
        {
            messenger.Send(
                this,
                new SwitchViewMessage
                {
                    NextView = typeof(AuthenticationViewModel)
                });
        }

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
                new SetupSecurityRequestMessage<RegistrationState>
                {
                    ReturnToViewModel = typeof(RegistrationViewModel),
                    State = new RegistrationState
                    {
                        Login = Login,
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

            try
            {
                messenger.Send(
                    this,
                    new SwitchLoadingIndicatorMessage
                    {
                        Switcher = true
                    });

                messenger.Send(
                    this,
                    new SwitchOffClosing
                    {
                        Switcher = true
                    });

                await Task.Run(() => userService.CreateAsync(storageProvider, Login, Password)).ConfigureAwait(false);

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
            }
            catch (UserExistsException)
            {
                AddError(nameof(Login), Errors.UserExists);
            }
            finally
            {
                messenger.Send(
                    this,
                    new SwitchLoadingIndicatorMessage
                    {
                        Switcher = false
                    });
            }
        }

        private void OnStateChangeRequested(RegistrationState state)
        {
            login = state.Login;
            password = state.Password;
        }

        private void OnSetupSecurityResponseReceived(SetupSecurityResponseMessage<RegistrationState> response)
        {
            OnStateChangeRequested(response.State);
            storageProvider = response.StorageProvider;
        }

        #endregion Event handlers

        #region Private methods

        private void AddValidation()
        {
            AddRule(nameof(Login), () => !string.IsNullOrWhiteSpace(Login), Errors.LoginEmpty, true);
            AddRule(nameof(Login), () => Regex.IsMatch(Login ?? string.Empty, "^[a-zA-Z0-9]*$"), Errors.LoginFormatInvalid);
            AddRule(nameof(Password), () => !string.IsNullOrEmpty(Password), Errors.PasswordEmpty, true);
        }

        #endregion Private methods
    }
}