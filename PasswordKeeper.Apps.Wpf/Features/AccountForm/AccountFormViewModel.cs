using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using PasswordKeeper.Apps.Communication;
using PasswordKeeper.Apps.Wpf.Abstractions;
using PasswordKeeper.Apps.Wpf.Features.Accounts;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.Requests;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.Responses;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.States;
using PasswordKeeper.Apps.Wpf.Messages.Switchers;
using PasswordKeeper.Apps.Wpf.Properties;
using PasswordKeeper.Services.Abstractions;
using PasswordKeeper.Services.Entities;

namespace PasswordKeeper.Apps.Wpf.Features.AccountForm
{
    public class AccountFormViewModel : ValidatableViewModel, INotifyPropertyChanged, ISubscribable
    {
        private readonly IMessenger messenger;
        private readonly IUserService userService;

        private AccountsState accountsState;
        private Account account;

        public AccountFormViewModel(IUserService userService, ICommandFactory commandFactory, IMessenger messenger)
        {
            this.userService = userService;
            this.messenger = messenger;

            SaveButtonClick = commandFactory.Create(OnSaveButtonClick);
            BackButtonClick = commandFactory.Create(OnBackButtonClick);
            GenerateButtonClick = commandFactory.Create(OnGenerateButtonClick);

            AddValidation();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region View properties

        public bool IsAdd { get; set; }

        public bool IsEdit { get => !IsAdd; }

        public string AccountName
        {
            get
            {
                return account.AccountName;
            }

            set
            {
                account.AccountName = value;
                Validate(nameof(AccountName));
            }
        }

        public string Login
        {
            get
            {
                return account.Login;
            }

            set
            {
                account.Login = value;
                Validate(nameof(Login));
            }
        }

        public string Password
        {
            get
            {
                return account.Password;
            }

            set
            {
                account.Password = value;
                Validate(nameof(Password));
            }
        }

        public bool IsHidden
        {
            get => account.IsHidden;
            set => account.IsHidden = value;
        }

        public string Notes
        {
            get => account.Notes;
            set => account.Notes = value;
        }

        #endregion View properties

        #region Commands

        public ICommand BackButtonClick { get; }

        public ICommand SaveButtonClick { get; }

        public ICommand GenerateButtonClick { get; }

        #endregion Commands

        #region Private properties

        private User User { get => userService.User; }

        #endregion Private properties

        #region Public methods

        public void Subscribe()
        {
            messenger.Subscribe<AccountAddRequestMessage<AccountsState>>(this, OnAddRequestReceived);
            messenger.Subscribe<AccountEditRequestMessage<AccountsState>>(this, OnEditRequestReceived);
            messenger.Subscribe<GeneratePasswordResponseMessage<AccountFormState>>(this, OnGeneratePasswordResponseReceived);
        }

        public void Unsubscribe()
        {
            messenger.Unsubscribe<AccountAddRequestMessage<AccountsState>>(this);
            messenger.Unsubscribe<AccountEditRequestMessage<AccountsState>>(this);
            messenger.Unsubscribe<GeneratePasswordResponseMessage<AccountFormState>>(this);
        }

        #endregion Public methods

        #region Event handlers

        private void OnBackButtonClick(object obj)
        {
            messenger.Send(
                this,
                new SwitchViewMessage
                {
                    NextView = typeof(AccountsViewModel)
                });

            messenger.Send(
                this,
                accountsState);
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

            User.RemoveAccount(account.Id);
            User.AddAccount(account);

            await Task.Run(() => userService.SaveAsync()).ConfigureAwait(false);

            messenger.Send(
                this,
                new SwitchViewMessage
                {
                    NextView = typeof(AccountsViewModel)
                });

            messenger.Send(
                this,
                new AccountsState
                {
                    SelectedAccount = account,
                    SearchText = accountsState.SearchText
                });

            messenger.Send(
                this,
                new SwitchLoadingIndicatorMessage
                {
                    Switcher = false
                });
        }

        private void OnGenerateButtonClick(object obj)
        {
            messenger.Send(
                this,
                new SwitchViewMessage
                {
                    NextView = typeof(PasswordGenerationViewModel)
                });

            messenger.Send(
                this,
                new GeneratePasswordRequestMessage<AccountFormState>
                {
                    State = new AccountFormState
                    {
                        Account = account,
                        AccountsState = accountsState
                    }
                });
        }

        private void OnAddRequestReceived(AccountAddRequestMessage<AccountsState> message)
        {
            SwitchIsAdd(true);
            accountsState = message.State;
            account = new Account
            {
                Id = Guid.NewGuid()
            };
        }

        private void OnEditRequestReceived(AccountEditRequestMessage<AccountsState> message)
        {
            SwitchIsAdd(false);
            accountsState = message.State;
            account = new Account(message.State.SelectedAccount);
        }

        private void OnGeneratePasswordResponseReceived(GeneratePasswordResponseMessage<AccountFormState> message)
        {
            accountsState = message.State.AccountsState;
            account = message.State.Account;

            account.Password = message.GeneratedPassword ?? account.Password;
        }

        #endregion Event handlers

        #region Private methods

        private void AddValidation()
        {
            AddRule(nameof(AccountName), () => !string.IsNullOrEmpty(AccountName), Errors.AccountNameEmpty);
            AddRule(nameof(Login), () => !string.IsNullOrEmpty(Login), Errors.LoginEmpty);
            AddRule(nameof(Password), () => !string.IsNullOrEmpty(Password), Errors.PasswordEmpty);
        }

        private void SwitchIsAdd(bool switcher)
        {
            IsAdd = switcher;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAdd)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEdit)));
        }

        #endregion Private methods
    }
}