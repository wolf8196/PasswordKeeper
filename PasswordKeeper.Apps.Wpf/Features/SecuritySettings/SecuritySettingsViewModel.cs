using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PasswordKeeper.Apps.Communication;
using PasswordKeeper.Apps.Wpf.Abstractions;
using PasswordKeeper.Apps.Wpf.Features.SecuritySettings.Kdfs;
using PasswordKeeper.Apps.Wpf.Features.SecuritySettings.Kdfs.Pbkdf2;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.Requests;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.Responses;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.States;
using PasswordKeeper.Apps.Wpf.Messages.Switchers;
using PasswordKeeper.Apps.Wpf.Properties;
using PasswordKeeper.Services.CryptoProviders;
using PasswordKeeper.Services.Enums;

namespace PasswordKeeper.Apps.Wpf.Features.SecuritySettings
{
    public class SecuritySettingsViewModel : ViewModelBase, INotifyPropertyChanged, ISubscribable
    {
        private readonly ICommandFactory commandFactory;
        private readonly IMessenger messenger;
        private readonly IViewModelFactory viewModelFactory;

        private readonly ConfigurableStorageProvider storageProvider;

        private KeyTransformations selectedKdf;
        private string message;

        public SecuritySettingsViewModel(ICommandFactory commandFactory, IMessenger messenger, IViewModelFactory viewModelFactory)
        {
            this.commandFactory = commandFactory;
            this.messenger = messenger;
            this.viewModelFactory = viewModelFactory;

            storageProvider = new ConfigurableStorageProvider();

            RunOneSecondSetupButtonClick = commandFactory.Create(OnRunOneSecondSetupButtonClickAsync);
            RunTestButtonClick = commandFactory.Create(OnRunTestButtonClick);

            SelectedEncryptionAlg = EncryptionAlgorithms.Aes256CbcHmacSha512;
            SelectedKdf = KeyTransformations.Pbkdf2;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region View properties

        public KdfViewModelBase KdfViewModel { get; set; }

        public List<EncryptionAlgorithms> EncryptionAlgs
        {
            get
            {
                return Enum.GetValues(typeof(EncryptionAlgorithms)).Cast<EncryptionAlgorithms>().ToList();
            }
        }

        public EncryptionAlgorithms SelectedEncryptionAlg
        {
            get
            {
                return storageProvider.EncryptionAlgorithm;
            }

            set
            {
                storageProvider.EncryptionAlgorithm = value;
            }
        }

        public List<KeyTransformations> Kdfs
        {
            get
            {
                return Enum.GetValues(typeof(KeyTransformations)).Cast<KeyTransformations>().ToList();
            }
        }

        public KeyTransformations SelectedKdf
        {
            get
            {
                return selectedKdf;
            }

            set
            {
                selectedKdf = value;
                SetKdfViewModel();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedKdf)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(KdfViewModel)));
            }
        }

        public string Message
        {
            get
            {
                return message;
            }

            set
            {
                message = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Message)));
            }
        }

        #endregion View properties

        #region Commands

        public ICommand BackButtonClick { get; private set; }

        public ICommand ConfirmButtonClick { get; private set; }

        public ICommand SaveButtonClick { get; }

        public ICommand RunOneSecondSetupButtonClick { get; }

        public ICommand RunTestButtonClick { get; }

        #endregion Commands

        #region Public methods

        public void Subscribe()
        {
            messenger.Subscribe<SetupSecurityRequestMessage<RegistrationState>>(this, OnStateChangeRequested);
            messenger.Subscribe<SetupSecurityRequestMessage<KeyChangeState>>(this, OnStateChangeRequested);
        }

        public void Unsubscribe()
        {
            messenger.Unsubscribe<SetupSecurityRequestMessage<RegistrationState>>(this);
            messenger.Unsubscribe<SetupSecurityRequestMessage<KeyChangeState>>(this);
        }

        #endregion Public methods

        #region Event handlers

        private void OnStateChangeRequested<T>(SetupSecurityRequestMessage<T> message)
        {
            BackButtonClick = commandFactory.Create((obj) =>
            {
                OnBackButtonClick(message);
            });

            ConfirmButtonClick = commandFactory.Create((obj) =>
            {
                OnConfirmButtonClick(message);
            });
        }

        private void OnBackButtonClick<T>(SetupSecurityRequestMessage<T> message)
        {
            messenger.Send(
                this,
                new SwitchViewMessage
                {
                    NextView = message.ReturnToViewModel
                });

            messenger.Send(this, message.State);
        }

        private void OnConfirmButtonClick<T>(SetupSecurityRequestMessage<T> message)
        {
            storageProvider.KdfProvider = KdfViewModel.GetKdfProvider();

            messenger.Send(
                this,
                new SwitchViewMessage
                {
                    NextView = message.ReturnToViewModel
                });

            messenger.Send(
                this,
                new SetupSecurityResponseMessage<T>
                {
                    State = message.State,
                    StorageProvider = storageProvider
                });
        }

        private async void OnRunOneSecondSetupButtonClickAsync(object obj)
        {
            Message = null;
            messenger.Send(
                this,
                new SwitchLoadingIndicatorMessage
                {
                    Switcher = true
                });

            await Task.Run(() => KdfViewModel.RunOneSecondSetup()).ConfigureAwait(false);

            messenger.Send(
                this,
                new SwitchLoadingIndicatorMessage
                {
                    Switcher = false
                });
        }

        private async void OnRunTestButtonClick(object obj)
        {
            Message = null;
            messenger.Send(
                this,
                new SwitchLoadingIndicatorMessage
                {
                    Switcher = true
                });

            var elapsed = await Task.Run(() => KdfViewModel.RunTest()).ConfigureAwait(false);

            Message = string.Format(Templates.PasswordDerivedMessage, elapsed.TotalSeconds);

            messenger.Send(
                this,
                new SwitchLoadingIndicatorMessage
                {
                    Switcher = false
                });
        }

        #endregion Event handlers

        #region Private methods

        private void SetKdfViewModel()
        {
            switch (SelectedKdf)
            {
                case KeyTransformations.Pbkdf2:
                    KdfViewModel = viewModelFactory.Create<Pbkdf2ViewModel>();
                    break;

                default:
                    throw new NotImplementedException($"No implementation found for KeyTransformations: {SelectedKdf}");
            }
        }

        #endregion Private methods
    }
}