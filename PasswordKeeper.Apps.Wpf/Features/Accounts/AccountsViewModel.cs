using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PasswordKeeper.Apps.Communication;
using PasswordKeeper.Apps.Wpf.Abstractions;
using PasswordKeeper.Apps.Wpf.Abstractions.Services;
using PasswordKeeper.Apps.Wpf.Enums;
using PasswordKeeper.Apps.Wpf.Features.AccountForm;
using PasswordKeeper.Apps.Wpf.Features.Attachments;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.Requests;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.Responses;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.States;
using PasswordKeeper.Apps.Wpf.Messages.Switchers;
using PasswordKeeper.Apps.Wpf.Properties;
using PasswordKeeper.Services.Abstractions;
using PasswordKeeper.Services.Entities;

namespace PasswordKeeper.Apps.Wpf.Features.Accounts
{
    public class AccountsViewModel : ViewModelBase, INotifyPropertyChanged, ISubscribable
    {
        private readonly IMessenger messenger;
        private readonly IUserService userService;
        private readonly IAttachmentService attachmentService;
        private readonly IFileProcessor fileProcessor;

        private Account selectedAccount;
        private string searchText;
        private bool showPassword;
        private bool isDragOver;

        private Guid dialogId;

        public AccountsViewModel(
            IUserService userService,
            IAttachmentService attachmentService,
            ICommandFactory commandFactory,
            IMessenger messenger,
            IFileProcessor fileProcessor)
        {
            this.userService = userService;
            this.attachmentService = attachmentService;
            this.messenger = messenger;
            this.fileProcessor = fileProcessor;

            AddButtonClick = commandFactory.Create(OnAddButtonClick);
            EditButtonClick = commandFactory.Create(OnEditButtonClick);
            DeleteButtonClick = commandFactory.Create(OnDeleteButtonClick);
            AttachmentsButtonClick = commandFactory.Create(OnAttachmentsButtonClick);
            WatchButtonMouseDown = commandFactory.Create(OnWatchButtonMouseDown);
            WatchButtonMouseUp = commandFactory.Create(OnWatchButtonMouseUp);
            PasswordCopyButtonClick = commandFactory.Create(OnPasswordCopyButtonClick);
            LoginCopyButtonClick = commandFactory.Create(OnLoginCopyButtonClick);
            SearchReset = commandFactory.Create(OnSearchReset);
            MenuButtonClick = commandFactory.Create(OnMenuButtonClick);
            DragEnter = commandFactory.Create(OnDragEnter);
            DragLeave = commandFactory.Create(OnDragLeave);
            FileDrop = commandFactory.Create(
                (obj) =>
                {
                    fileProcessor.ProcessFiles(obj as string[]);
                    IsDragOver = false;
                });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region View properties

        public List<Account> Accounts
        {
            get
            {
                return User.Accounts
                    .Where(x => string.IsNullOrEmpty(searchText?.Trim()) || x.AccountName.ToLower().Contains(searchText.Trim().ToLower()))
                    .Where(x => User.Settings.ShowHiddenAccounts || !x.IsHidden)
                    .Select(x => x)
                    .ToList();
            }
        }

        public bool IsAccountSelected => selectedAccount != null;

        public Account SelectedAccount
        {
            get
            {
                return selectedAccount;
            }

            set
            {
                selectedAccount = value;
                fileProcessor.SelectedAccount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAccount)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAccountSelected)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowPasswordMask)));
            }
        }

        public bool ShowPassword
        {
            get
            {
                return showPassword;
            }

            set
            {
                showPassword = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowPassword)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowPasswordMask)));
            }
        }

        public bool ShowPasswordMask => IsAccountSelected && !ShowPassword;

        public string SearchText
        {
            get
            {
                return searchText;
            }

            set
            {
                searchText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Accounts)));
            }
        }

        public bool IsDragOver
        {
            get
            {
                return isDragOver && IsAccountSelected;
            }

            set
            {
                isDragOver = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDragOver)));
            }
        }

        #endregion View properties

        #region Commands

        public ICommand AddButtonClick { get; }

        public ICommand DeleteButtonClick { get; }

        public ICommand EditButtonClick { get; }

        public ICommand AttachmentsButtonClick { get; }

        public ICommand LoginCopyButtonClick { get; }

        public ICommand PasswordCopyButtonClick { get; }

        public ICommand WatchButtonMouseDown { get; }

        public ICommand WatchButtonMouseUp { get; }

        public ICommand SearchReset { get; }

        public ICommand MenuButtonClick { get; }

        public ICommand DragEnter { get; }

        public ICommand DragLeave { get; }

        public ICommand FileDrop { get; }

        #endregion Commands

        #region Private properties

        private User User { get => userService.User; }

        #endregion Private properties

        #region Public methods

        public void Subscribe()
        {
            messenger.Subscribe<AccountsState>(this, OnStateChangeRequested);
            messenger.Subscribe<ConfirmDialogResponseMessage>(this, OnConfirmDialogResponseReceived);
        }

        public void Unsubscribe()
        {
            messenger.Unsubscribe<AccountsState>(this);
            messenger.Unsubscribe<ConfirmDialogResponseMessage>(this);

            fileProcessor.Unsubscribe();
        }

        #endregion Public methods

        #region Event handlers

        private void OnAddButtonClick(object obj)
        {
            messenger.Send(
                this,
                new SwitchViewMessage
                {
                    NextView = typeof(AccountFormViewModel)
                });

            messenger.Send(
                this,
                new AccountAddRequestMessage<AccountsState>
                {
                    State = new AccountsState
                    {
                        SelectedAccount = selectedAccount,
                        SearchText = searchText
                    }
                });
        }

        private void OnEditButtonClick(object obj)
        {
            messenger.Send(
                this,
                new SwitchViewMessage
                {
                    NextView = typeof(AccountFormViewModel)
                });

            messenger.Send(
                this,
                new AccountEditRequestMessage<AccountsState>
                {
                    State = new AccountsState
                    {
                        SelectedAccount = selectedAccount,
                        SearchText = searchText
                    }
                });
        }

        private void OnDeleteButtonClick(object obj)
        {
            dialogId = Guid.NewGuid();
            messenger.Send(
                this,
                new ConfirmDialogRequestMessage
                {
                    DialogId = dialogId,
                    Title = Titles.Delete,
                    Message = string.Format(Templates.DeleteMessage, SelectedAccount.AccountName),
                    DialogMode = DialogModes.OkCancel,
                    OnResponseReceived = OnDeleteResponseReceived
                });
        }

        private void OnLoginCopyButtonClick(object obj)
        {
            if (IsAccountSelected)
            {
                Clipboard.SetText(selectedAccount.Login);
            }
        }

        private void OnPasswordCopyButtonClick(object obj)
        {
            if (IsAccountSelected)
            {
                Clipboard.SetText(selectedAccount.Password);
            }
        }

        private void OnWatchButtonMouseDown(object obj)
        {
            ShowPassword = true;
        }

        private void OnWatchButtonMouseUp(object obj)
        {
            ShowPassword = false;
        }

        private void OnSearchReset(object obj)
        {
            SearchText = string.Empty;
        }

        private void OnMenuButtonClick(object obj)
        {
            messenger.Send(
                this,
                new SwitchMenuMessage
                {
                    Switcher = true
                });
        }

        private void OnDragEnter(object obj)
        {
            if (!IsDragOver)
            {
                IsDragOver = true;
            }
        }

        private void OnDragLeave(object obj)
        {
            if (IsDragOver)
            {
                IsDragOver = false;
            }
        }

        private void OnAttachmentsButtonClick(object obj)
        {
            messenger.Send(
                this,
                new SwitchViewMessage
                {
                    NextView = typeof(AttachmentsViewModel)
                });

            messenger.Send(
                this,
                new AttachmentsRequestMessage<AccountsState>
                {
                    State = new AccountsState
                    {
                        SelectedAccount = selectedAccount,
                        SearchText = searchText
                    }
                });
        }

        private void OnStateChangeRequested(AccountsState message)
        {
            SearchText = message.SearchText;
            SelectedAccount = Accounts.FirstOrDefault(x => x == message.SelectedAccount);
        }

        private async void OnConfirmDialogResponseReceived(ConfirmDialogResponseMessage message)
        {
            if (message != null && message.DialogId == dialogId)
            {
                await message.OnResponseReceived(message.Response).ConfigureAwait(false);
            }
        }

        private async Task OnDeleteResponseReceived(bool response)
        {
            if (response)
            {
                messenger.Send(
                    this,
                    new SwitchLoadingIndicatorMessage
                    {
                        Switcher = true
                    });

                await attachmentService.RemoveAllAsync(SelectedAccount).ConfigureAwait(false);

                User.RemoveAccount(SelectedAccount);

                messenger.Send(
                    this,
                    new SwitchOffClosing
                    {
                        Switcher = true
                    });

                await userService.SaveAsync().ConfigureAwait(false);

                messenger.Send(
                    this,
                    new SwitchOffClosing
                    {
                        Switcher = false
                    });

                SelectedAccount = null;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Accounts)));

                messenger.Send(
                    this,
                    new SwitchLoadingIndicatorMessage
                    {
                        Switcher = false
                    });
            }
        }

        #endregion Event handlers
    }
}