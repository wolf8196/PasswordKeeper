using System;
using System.Threading.Tasks;
using System.Windows.Input;
using PasswordKeeper.Apps.Communication;
using PasswordKeeper.Apps.Wpf.Abstractions;
using PasswordKeeper.Apps.Wpf.Features.Accounts;
using PasswordKeeper.Apps.Wpf.Features.Registration;
using PasswordKeeper.Apps.Wpf.Messages.Switchers;
using PasswordKeeper.Apps.Wpf.Properties;
using PasswordKeeper.Services.Abstractions;
using PasswordKeeper.Services.Exceptions;

namespace PasswordKeeper.Apps.Wpf.Features.Authentication
{
    public class AuthenticationViewModel : ValidatableViewModel
    {
        private readonly IMessenger messenger;
        private readonly IUserService userService;

        private string login;
        private string password;

        public AuthenticationViewModel(IUserService userService, ICommandFactory commandFactory, IMessenger messenger)
        {
            this.userService = userService;
            this.messenger = messenger;

            RegisterButtonClick = commandFactory.Create(OnRegistrationButtonClick);
            LogInButtonClick = commandFactory.Create(OnLogInButtonClick);

            AddValidation();
        }

        #region View properties

        public string Login
        {
            get
            {
                return login;
            }

            set
            {
                login = value;
                Validate(nameof(Login));
            }
        }

        public string Password
        {
            get
            {
                return password;
            }

            set
            {
                password = value;
                Validate(nameof(Password));
            }
        }

        #endregion View properties

        #region Commands

        public ICommand LogInButtonClick { get; }

        public ICommand RegisterButtonClick { get; }

        #endregion Commands

        #region Event handlers

        private async void OnLogInButtonClick(object obj)
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

                await Task.Run(() => userService.LogInAsync(Login, Password)).ConfigureAwait(false);

                messenger.Send(
                    this,
                    new SwitchViewMessage
                    {
                        NextView = typeof(AccountsViewModel)
                    });
            }
            catch (UserNotFoundException)
            {
                AddError(nameof(Login), Errors.UserNotFound);
            }
            catch (UserLoggedException)
            {
                AddError(nameof(Login), Errors.UserLogged);
            }
            catch (AuthenticationException)
            {
                AddError(nameof(Password), Errors.InvalidPassword);
            }
            catch (FormatException)
            {
                AddError(nameof(Login), Errors.InvalidDb);
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

        private void OnRegistrationButtonClick(object obj)
        {
            messenger.Send(
                this,
                new SwitchViewMessage
                {
                    NextView = typeof(RegistrationViewModel)
                });
        }

        #endregion Event handlers

        #region Private methods

        private void AddValidation()
        {
            AddRule(nameof(Login), () => !string.IsNullOrWhiteSpace(Login), Errors.LoginEmpty);
            AddRule(nameof(Password), () => !string.IsNullOrEmpty(Password), Errors.PasswordEmpty);
        }

        #endregion Private methods
    }
}