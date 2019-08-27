using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using PasswordKeeper.Apps.Communication;
using PasswordKeeper.Apps.Wpf.Abstractions;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.Requests;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.Responses;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.States;
using PasswordKeeper.Apps.Wpf.Messages.Switchers;
using PasswordKeeper.Services.Abstractions;
using PasswordKeeper.Services.Models;

namespace PasswordKeeper.Apps.Wpf.Features.AccountForm
{
    public class PasswordGenerationViewModel : ViewModelBase, INotifyPropertyChanged, ISubscribable
    {
        private readonly IMessenger messenger;
        private readonly IGenerator<string, PasswordGenerationParameters> generator;

        private AccountFormState accountFormState;
        private string generatedPassword;

        public PasswordGenerationViewModel(
            ICommandFactory commandFactory,
            IMessenger messenger,
            IGenerator<string, PasswordGenerationParameters> generator)
        {
            this.messenger = messenger;
            this.generator = generator;

            ConfirmButtonClick = commandFactory.Create(OnConfirmButtonClick);
            BackButtonClick = commandFactory.Create(OnBackButtonClick);
            GenerateButtonClick = commandFactory.Create(OnGenerateButtonClick);

            LengthValues = new List<int>();
            PopulateLength();

            Parameters = new PasswordGenerationParameters
            {
                Length = LengthValues.Find(x => x == 16),
                IncludeSpecialCharacters = false,
                IncludeNumericCharacters = true,
                IncludeLowerCaseCharacters = true,
                IncludeUpperCaseCharacters = true,
                ExcludeSimilarCharacters = true,
                ExcludeAmbiguousCharacters = true
            };
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Parameters)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region View properties

        public List<int> LengthValues { get; }

        public PasswordGenerationParameters Parameters { get; set; }

        public string GeneratedPassword
        {
            get
            {
                return generatedPassword;
            }

            set
            {
                generatedPassword = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GeneratedPassword)));
            }
        }

        #endregion View properties

        #region Commands

        public ICommand BackButtonClick { get; }

        public ICommand ConfirmButtonClick { get; }

        public ICommand GenerateButtonClick { get; }

        #endregion Commands

        #region Public methods

        public void Subscribe()
        {
            messenger.Subscribe<GeneratePasswordRequestMessage<AccountFormState>>(this, OnGenerationRequested);
        }

        public void Unsubscribe()
        {
            messenger.Unsubscribe<GeneratePasswordRequestMessage<AccountFormState>>(this);
        }

        #endregion Public methods

        #region Event handlers

        private void OnBackButtonClick(object obj)
        {
            messenger.Send(
                this,
                new SwitchViewMessage
                {
                    NextView = typeof(AccountFormViewModel)
                });

            messenger.Send(
                this,
                new GeneratePasswordResponseMessage<AccountFormState>
                {
                    State = accountFormState
                });
        }

        private void OnConfirmButtonClick(object obj)
        {
            messenger.Send(
                this,
                new SwitchViewMessage
                {
                    NextView = typeof(AccountFormViewModel)
                });

            messenger.Send(
                this,
                new GeneratePasswordResponseMessage<AccountFormState>
                {
                    State = accountFormState,
                    GeneratedPassword = GeneratedPassword
                });
        }

        private void OnGenerateButtonClick(object obj)
        {
            GeneratedPassword = generator.Generate(Parameters);
        }

        private void OnGenerationRequested(GeneratePasswordRequestMessage<AccountFormState> message)
        {
            accountFormState = message.State;
        }

        #endregion Event handlers

        private void PopulateLength()
        {
            LengthValues.Clear();
            int i;
            for (i = 6; i < 128; ++i)
            {
                LengthValues.Add(i);
            }

            while (i <= 2048)
            {
                LengthValues.Add(i);
                i *= 2;
            }
        }
    }
}