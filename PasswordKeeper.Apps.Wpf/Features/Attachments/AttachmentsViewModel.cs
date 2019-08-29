using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PasswordKeeper.Apps.Communication;
using PasswordKeeper.Apps.Wpf.Abstractions;
using PasswordKeeper.Apps.Wpf.Abstractions.Adapters;
using PasswordKeeper.Apps.Wpf.Abstractions.Services;
using PasswordKeeper.Apps.Wpf.Enums;
using PasswordKeeper.Apps.Wpf.Features.Accounts;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.Requests;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.Responses;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.States;
using PasswordKeeper.Apps.Wpf.Messages.Switchers;
using PasswordKeeper.Apps.Wpf.Properties;
using PasswordKeeper.Services.Abstractions;
using PasswordKeeper.Services.Entities;

namespace PasswordKeeper.Apps.Wpf.Features.Attachments
{
    public class AttachmentsViewModel : ViewModelBase, INotifyPropertyChanged, ISubscribable
    {
        private readonly IMessenger messenger;
        private readonly IAttachmentService attachmentService;
        private readonly IFolderBrowserDialogAdapter folderDialog;
        private readonly IFileProcessor fileProcessor;

        private AccountsState accountsState;
        private Attachment selectedAttachment;
        private bool isDragOver;

        private Guid dialogId;

        public AttachmentsViewModel(
            IAttachmentService attachmentService,
            ICommandFactory commandFactory,
            IMessenger messenger,
            IFolderBrowserDialogAdapter folderDialog,
            IFileProcessor fileProcessor)
        {
            this.attachmentService = attachmentService;
            this.messenger = messenger;
            this.folderDialog = folderDialog;
            this.fileProcessor = fileProcessor;
            this.fileProcessor.ProcessingFinished +=
                () => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Attachments)));

            BackButtonClick = commandFactory.Create(OnBackButtonClick);
            ExtractButtonClick = commandFactory.Create(OnExtractButtonClick);
            DeleteButtonClick = commandFactory.Create(OnDeleteButtonClick);
            DragEnter = commandFactory.Create(OnDragEnter);
            DragLeave = commandFactory.Create(OnDragLeave);
            FileDrop = commandFactory.Create(
                async (obj) =>
                {
                    await fileProcessor.ProcessFiles(obj as string[]).ConfigureAwait(false);
                    IsDragOver = false;
                });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region View properties

        public List<Attachment> Attachments
        {
            get
            {
                return accountsState.SelectedAccount.Attachments.Select(x => x).ToList();
            }
        }

        public string AccountName
        {
            get
            {
                return accountsState.SelectedAccount.AccountName;
            }
        }

        public Attachment SelectedAttachment
        {
            get
            {
                return selectedAttachment;
            }

            set
            {
                selectedAttachment = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAttachment)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAttachmentSelected)));
            }
        }

        public bool IsAttachmentSelected
        {
            get
            {
                return selectedAttachment != null;
            }
        }

        public bool IsDragOver
        {
            get
            {
                return isDragOver || !accountsState.SelectedAccount.Attachments.Any();
            }

            set
            {
                isDragOver = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDragOver)));
            }
        }

        #endregion View properties

        #region Commands

        public ICommand BackButtonClick { get; }

        public ICommand ExtractButtonClick { get; }

        public ICommand DeleteButtonClick { get; }

        public ICommand DragEnter { get; }

        public ICommand DragLeave { get; }

        public ICommand FileDrop { get; }

        #endregion Commands

        #region Public methods

        public void Subscribe()
        {
            messenger.Subscribe<AttachmentsRequestMessage<AccountsState>>(this, OnAttachmentsRequestReceived);
            messenger.Subscribe<ConfirmDialogResponseMessage>(this, OnConfirmDialogResponseReceived);
        }

        public void Unsubscribe()
        {
            messenger.Unsubscribe<AttachmentsRequestMessage<AccountsState>>(this);
            messenger.Unsubscribe<ConfirmDialogResponseMessage>(this);
            fileProcessor.Unsubscribe();
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

        private void OnAttachmentsRequestReceived(AttachmentsRequestMessage<AccountsState> message)
        {
            accountsState = message.State;
            fileProcessor.SelectedAccount = message.State.SelectedAccount;
        }

        private async void OnExtractButtonClick(object obj)
        {
            var extractDirectory = folderDialog.ShowDialog();

            if (extractDirectory == null)
            {
                return;
            }

            messenger.Send(
                this,
                new SwitchLoadingIndicatorMessage
                {
                    Switcher = true
                });

            try
            {
                await Task.Run(async () =>
                {
                    var path = attachmentService.GetExtractPath(Path.Combine(extractDirectory, SelectedAttachment.Name));

                    await attachmentService.GetAsync(SelectedAttachment, path).ConfigureAwait(false);
                }).ConfigureAwait(false);
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

        private void OnDeleteButtonClick(object obj)
        {
            dialogId = Guid.NewGuid();
            messenger.Send(
                this,
                new ConfirmDialogRequestMessage
                {
                    DialogId = dialogId,
                    Title = Titles.Delete,
                    Message = string.Format(Templates.AttachmentDeleteMessage, SelectedAttachment.Name),
                    DialogMode = DialogModes.OkCancel,
                    OnResponseReceived = OnDeleteResponseReceived
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

                await attachmentService.RemoveAsync(accountsState.SelectedAccount, SelectedAttachment).ConfigureAwait(false);

                SelectedAttachment = null;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Attachments)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDragOver)));

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