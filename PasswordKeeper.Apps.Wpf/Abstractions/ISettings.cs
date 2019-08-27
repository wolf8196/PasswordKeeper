namespace PasswordKeeper.Apps.Wpf.Abstractions
{
    public interface ISettings
    {
        bool IsFirstLaunch { get; set; }

        string UserDirectoryName { get; set; }

        string UserDirectoryLocation { get; set; }
    }
}