namespace PasswordKeeper.Apps.Wpf.Messages.Navigation.Responses
{
    public class ConfirmDialogResponseMessage : DialogMessageBase<bool>
    {
        public bool Response { get; set; }
    }
}