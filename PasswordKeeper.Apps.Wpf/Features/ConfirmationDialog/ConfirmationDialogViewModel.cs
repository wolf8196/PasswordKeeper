using System.ComponentModel;
using System.Windows.Input;
using PasswordKeeper.Apps.Communication;
using PasswordKeeper.Apps.Wpf.Abstractions;
using PasswordKeeper.Apps.Wpf.Enums;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.Requests;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.Responses;

namespace PasswordKeeper.Apps.Wpf.Features.ConfirmationDialog
{
    public class ConfirmationDialogViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly IMessenger messenger;

        private ConfirmDialogRequestMessage request;

        public ConfirmationDialogViewModel(ICommandFactory commandFactory, IMessenger messenger)
        {
            this.messenger = messenger;

            OkButtonClick = commandFactory.Create(OnOkButtonClick);
            CancelButtonClick = commandFactory.Create(OnCancelButtonClick, (o) => IsOkCancelMode);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region View properties

        public ConfirmDialogRequestMessage Request
        {
            get
            {
                return request;
            }

            set
            {
                request = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Request)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOkMode)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOkCancelMode)));
            }
        }

        public bool IsOkMode
        {
            get
            {
                return Request.DialogMode == DialogModes.Ok;
            }
        }

        public bool IsOkCancelMode
        {
            get
            {
                return Request.DialogMode == DialogModes.OkCancel;
            }
        }

        #endregion View properties

        #region Commands

        public ICommand CancelButtonClick { get; }

        public ICommand OkButtonClick { get; }

        #endregion Commands

        #region Event handlers

        private void OnCancelButtonClick(object obj)
        {
            messenger.Send(
                this,
                new ConfirmDialogResponseMessage
                {
                    Response = false,
                    DialogId = Request.DialogId,
                    OnResponseReceived = Request.OnResponseReceived
                });
        }

        private void OnOkButtonClick(object obj)
        {
            messenger.Send(
                this,
                new ConfirmDialogResponseMessage
                {
                    Response = true,
                    DialogId = Request.DialogId,
                    OnResponseReceived = Request.OnResponseReceived
                });
        }

        #endregion Event handlers
    }
}