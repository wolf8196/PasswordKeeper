namespace PasswordKeeper.Apps.Wpf.Abstractions
{
    public interface ISubscribable
    {
        void Subscribe();

        void Unsubscribe();
    }
}