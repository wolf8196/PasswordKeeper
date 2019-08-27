using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PasswordKeeper.Apps.Communication;
using PasswordKeeper.Apps.Wpf.Abstractions.Services;
using PasswordKeeper.Apps.Wpf.Enums;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.Requests;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.Responses;
using PasswordKeeper.Apps.Wpf.Messages.Switchers;
using PasswordKeeper.Apps.Wpf.Properties;
using PasswordKeeper.Services.Abstractions;
using PasswordKeeper.Services.Entities;
using PasswordKeeper.Services.Exceptions;

namespace PasswordKeeper.Apps.Wpf.Services
{
    public class FileProcessor : IFileProcessor
    {
        private readonly IAttachmentService attachmentService;
        private readonly IMessenger messenger;

        private readonly Queue<string> attachmentsQueue;
        private Guid dialogId;

        public FileProcessor(
            IAttachmentService attachmentService,
            IMessenger messenger)
        {
            this.attachmentService = attachmentService;
            this.messenger = messenger;

            attachmentsQueue = new Queue<string>();

            Subscribe();
        }

        public event Action ProcessingFinished;

        public Account SelectedAccount { get; set; }

        public async Task ProcessFiles(string[] files)
        {
            if (files.Length != 0)
            {
                foreach (var item in files)
                {
                    attachmentsQueue.Enqueue(item);
                }

                await ProcessAddingQueue().ConfigureAwait(false);
            }
        }

        public void Subscribe()
        {
            messenger.Subscribe<ConfirmDialogResponseMessage>(this, ConfirmDialogResponseReceivedHandler);
        }

        public void Unsubscribe()
        {
            messenger.Unsubscribe<ConfirmDialogResponseMessage>(this);
        }

        private async Task ProcessAddingQueue()
        {
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

                while (attachmentsQueue.Count != 0)
                {
                    await attachmentService.AddAsync(SelectedAccount, attachmentsQueue.Peek()).ConfigureAwait(false);
                    attachmentsQueue.Dequeue();
                }

                messenger.Send(
                    this,
                    new SwitchOffClosing
                    {
                        Switcher = false
                    });

                messenger.Send(
                    this,
                    new SwitchLoadingIndicatorMessage
                    {
                        Switcher = false
                    });

                ProcessingFinished?.Invoke();
            }
            catch (AttachmentExistsException)
            {
                dialogId = Guid.NewGuid();

                messenger.Send(
                    this,
                    new SwitchOffClosing
                    {
                        Switcher = false
                    });

                messenger.Send(
                    this,
                    new SwitchLoadingIndicatorMessage
                    {
                        Switcher = false
                    });

                messenger.Send(
                    this,
                    new ConfirmDialogRequestMessage
                    {
                        DialogMode = DialogModes.OkCancel,
                        Title = Titles.Replace,
                        Message = string.Format(Templates.ReplaceMessage, Path.GetFileName(attachmentsQueue.Peek())),
                        DialogId = dialogId,
                        OnResponseReceived = ReplaceResponseReceivedHandler
                    });
            }
            catch (UnauthorizedAccessException)
            {
                NotifyUnauthorizedAccess();
            }
        }

        private async void ConfirmDialogResponseReceivedHandler(ConfirmDialogResponseMessage message)
        {
            if (message != null && message.DialogId == dialogId)
            {
                await message.OnResponseReceived(message.Response).ConfigureAwait(false);
            }
        }

        private async Task ReplaceResponseReceivedHandler(bool response)
        {
            if (response)
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

                try
                {
                    await attachmentService.ReplaceAsync(SelectedAccount, attachmentsQueue.Peek()).ConfigureAwait(false);

                    attachmentsQueue.Dequeue();

                    await ProcessAddingQueue().ConfigureAwait(false);

                    messenger.Send(
                        this,
                        new SwitchOffClosing
                        {
                            Switcher = false
                        });

                    messenger.Send(
                        this,
                        new SwitchLoadingIndicatorMessage
                        {
                            Switcher = false
                        });
                }
                catch (UnauthorizedAccessException)
                {
                    NotifyUnauthorizedAccess();

                    messenger.Send(
                        this,
                        new SwitchOffClosing
                        {
                            Switcher = false
                        });

                    messenger.Send(
                        this,
                        new SwitchLoadingIndicatorMessage
                        {
                            Switcher = false
                        });
                }
            }
            else
            {
                attachmentsQueue.Dequeue();
                await ProcessAddingQueue().ConfigureAwait(false);
            }
        }

        private void NotifyUnauthorizedAccess()
        {
            dialogId = Guid.NewGuid();

            messenger.Send(
                this,
                new SwitchLoadingIndicatorMessage
                {
                    Switcher = false
                });

            messenger.Send(
                this,
                new ConfirmDialogRequestMessage
                {
                    Title = Titles.AccessDenied,
                    DialogMode = DialogModes.Ok,
                    Message = string.Format(Templates.AccessDeniedMessage, Path.GetFileName(attachmentsQueue.Peek())),
                    DialogId = dialogId,
                    OnResponseReceived = async (response) =>
                    {
                        attachmentsQueue.Dequeue();
                        await ProcessAddingQueue().ConfigureAwait(false);
                    }
                });
        }
    }
}