namespace PasswordKeeper.Services.Entities
{
    public sealed class Settings
    {
        public Settings()
        {
        }

        public Settings(Settings settings)
        {
            ShowHiddenAccounts = settings.ShowHiddenAccounts;
            HideToTrayOnClose = settings.HideToTrayOnClose;
            HideToTrayOnMinimize = settings.HideToTrayOnMinimize;
            AlwaysOnTop = settings.AlwaysOnTop;
            StartWithWindows = settings.StartWithWindows;
        }

        public bool ShowHiddenAccounts { get; set; }

        public bool HideToTrayOnMinimize { get; set; }

        public bool HideToTrayOnClose { get; set; }

        public bool AlwaysOnTop { get; set; }

        public bool StartWithWindows { get; set; }
    }
}