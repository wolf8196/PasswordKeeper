using PasswordKeeper.Apps.Wpf.Enums;

namespace PasswordKeeper.Apps.Wpf.Messages.Navigation.Requests
{
    public class ConfirmDialogRequestMessage : DialogMessageBase<bool>
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public DialogModes DialogMode { get; set; }
    }
}