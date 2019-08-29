using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PasswordKeeper.Apps.Communication;
using PasswordKeeper.Apps.Wpf.Abstractions;
using PasswordKeeper.Apps.Wpf.Abstractions.Adapters;
using PasswordKeeper.Apps.Wpf.Features.Authentication;
using PasswordKeeper.Apps.Wpf.Features.ConfirmationDialog;
using PasswordKeeper.Apps.Wpf.Features.KeyChange;
using PasswordKeeper.Apps.Wpf.Features.LoadingIndicator;
using PasswordKeeper.Apps.Wpf.Features.UserSettings;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.Requests;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.Responses;
using PasswordKeeper.Apps.Wpf.Messages.Switchers;
using PasswordKeeper.Services.Abstractions;
using PasswordKeeper.Services.Entities;

namespace PasswordKeeper.Apps.Wpf.Features.Main
{
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged, ISubscribable
    {
        private readonly IViewModelFactory factory;
        private readonly IMessenger messenger;
        private readonly IUserService userService;
        private readonly IWindowStorage<ViewModelBase> windowStorage;

        private ViewModelBase primaryViewModel;
        private ViewModelBase secondaryViewModel;
        private bool isMenuOpened;
        private bool quit;
        private WindowState state;

        public MainViewModel(
            IUserService userService,
            IViewModelFactory factory,
            ICommandFactory commandFactory,
            IMessenger messenger,
            IWindowStorage<ViewModelBase> windowStorage)
        {
            this.userService = userService;
            this.factory = factory;
            this.messenger = messenger;
            this.windowStorage = windowStorage;

            CloseCommand = commandFactory.Create(OnClose);
            QuitCommand = commandFactory.Create(OnQuit);
            OpenCommand = commandFactory.Create(OnOpen);
            StateChangedCommand = commandFactory.Create(OnStateChanged);
            MenuButtonClick = commandFactory.Create(OnHideMenu);
            SettingsButtonClick = commandFactory.Create(OnSettingsClick);
            KeyChangeButtonClick = commandFactory.Create(OnKeyChangeClick);
            LogOutButtonClick = commandFactory.Create(OnLogOut);

            OnChangeView(new SwitchViewMessage
            {
                NextView = typeof(AuthenticationViewModel)
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region View properties

        public User User { get => userService.User; }

        public bool AlwaysOnTop { get => User?.Settings.AlwaysOnTop ?? false; }

        public bool IsMenuOpened
        {
            get
            {
                return isMenuOpened;
            }

            set
            {
                isMenuOpened = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMenuOpened)));
            }
        }

        public ViewModelBase PrimaryViewModel
        {
            get
            {
                return primaryViewModel;
            }

            set
            {
                primaryViewModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PrimaryViewModel)));
            }
        }

        public ViewModelBase SecondaryViewModel
        {
            get
            {
                return secondaryViewModel;
            }

            set
            {
                secondaryViewModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SecondaryViewModel)));
            }
        }

        #endregion View properties

        #region Commands

        public ICommand CloseCommand { get; }

        public ICommand QuitCommand { get; }

        public ICommand OpenCommand { get; }

        public ICommand StateChangedCommand { get; }

        public ICommand MenuButtonClick { get; }

        public ICommand SettingsButtonClick { get; }

        public ICommand KeyChangeButtonClick { get; }

        public ICommand LogOutButtonClick { get; }

        #endregion Commands

        #region Public methods

        public void Subscribe()
        {
            messenger.Subscribe<SwitchViewMessage>(this, OnChangeView);
            messenger.Subscribe<SwitchLoadingIndicatorMessage>(this, OnShowLoadingIndicator);
            messenger.Subscribe<SwitchMenuMessage>(this, OnSwitchMenu);
            messenger.Subscribe<ConfirmDialogRequestMessage>(this, OnShowConfirmDialog);
            messenger.Subscribe<ConfirmDialogResponseMessage>(this, OnDialogResponseReceived);
        }

        public void Unsubscribe()
        {
            messenger.Unsubscribe<SwitchViewMessage>(this);
            messenger.Unsubscribe<SwitchLoadingIndicatorMessage>(this);
            messenger.Unsubscribe<SwitchMenuMessage>(this);
            messenger.Unsubscribe<ConfirmDialogRequestMessage>(this);
            messenger.Unsubscribe<ConfirmDialogResponseMessage>(this);
        }

        #endregion Public methods

        #region Event handlers

        private void OnChangeView(SwitchViewMessage message)
        {
            var type = message.NextView;
            (primaryViewModel as ISubscribable)?.Unsubscribe();
            var nextViewModel = factory.Create(type);
            (nextViewModel as ISubscribable)?.Subscribe();
            PrimaryViewModel = nextViewModel;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(User)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlwaysOnTop)));
        }

        private void OnShowConfirmDialog(ConfirmDialogRequestMessage message)
        {
            var dialog = factory.Create<ConfirmationDialogViewModel>();
            SecondaryViewModel = dialog;
            dialog.Request = message;
        }

        private void OnDialogResponseReceived(object obj)
        {
            SecondaryViewModel = null;
        }

        private void OnShowLoadingIndicator(SwitchLoadingIndicatorMessage message)
        {
            SwitchLoadingIndicator(message.Switcher);
        }

        private void OnSwitchMenu(SwitchMenuMessage message)
        {
            IsMenuOpened = message.Switcher;
        }

        private void OnHideMenu(object obj)
        {
            IsMenuOpened = false;
        }

        private async void OnLogOut(object obj)
        {
            SwitchLoadingIndicator(true);

            OnHideMenu(obj);

            await Task.Run(() => userService.LogOutAsync()).ConfigureAwait(false);

            OnChangeView(new SwitchViewMessage
            {
                NextView = typeof(AuthenticationViewModel)
            });

            SwitchLoadingIndicator(false);
        }

        private void OnSettingsClick(object obj)
        {
            SwitchLoadingIndicator(true);

            OnHideMenu(obj);

            OnChangeView(new SwitchViewMessage
            {
                NextView = typeof(UserSettingsViewModel)
            });

            SwitchLoadingIndicator(false);
        }

        private void OnKeyChangeClick(object obj)
        {
            SwitchLoadingIndicator(true);

            OnHideMenu(obj);

            OnChangeView(new SwitchViewMessage
            {
                NextView = typeof(KeyChangeViewModel)
            });

            SwitchLoadingIndicator(false);
        }

        private void OnClose(object obj)
        {
            if (!(obj is CancelEventArgs args))
            {
                return;
            }

            if (!quit && (User?.Settings.HideToTrayOnClose).GetValueOrDefault())
            {
                windowStorage.GetWindow(this).Hide();
                args.Cancel = true;
            }
        }

        private void OnQuit(object obj)
        {
            quit = true;
            windowStorage.GetWindow(this).Close();
        }

        private void OnOpen(object obj)
        {
            var window = windowStorage.GetWindow(this);
            window.Show();
            window.State = state;
            window.Activate();
        }

        private void OnStateChanged(object obj)
        {
            var window = windowStorage.GetWindow(this);
            if (window.State == WindowState.Minimized)
            {
                if ((User?.Settings.HideToTrayOnMinimize).GetValueOrDefault())
                {
                    window.Hide();
                }
            }
            else
            {
                state = window.State;
            }
        }

        #endregion Event handlers

        #region Private methods

        private void SwitchLoadingIndicator(bool switcher)
        {
            SecondaryViewModel = switcher ? factory.Create<LoadingIndicatorViewModel>() : null;
        }

        #endregion Private methods
    }
}