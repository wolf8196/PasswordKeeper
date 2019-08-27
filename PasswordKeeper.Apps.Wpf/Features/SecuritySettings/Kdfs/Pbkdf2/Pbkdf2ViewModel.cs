using System;
using System.ComponentModel;
using PasswordKeeper.Services.Abstractions;
using PasswordKeeper.Services.CryptoProviders;

namespace PasswordKeeper.Apps.Wpf.Features.SecuritySettings.Kdfs.Pbkdf2
{
    public class Pbkdf2ViewModel : KdfViewModelBase, INotifyPropertyChanged
    {
        private readonly Pbkdf2Provider kdfProvider;

        public Pbkdf2ViewModel()
        {
            kdfProvider = new Pbkdf2Provider();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Iterations
        {
            get
            {
                return kdfProvider.Iterations;
            }

            set
            {
                kdfProvider.Iterations = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Iterations)));
            }
        }

        public override IKdfProvider GetKdfProvider()
        {
            return kdfProvider;
        }

        public override void RunOneSecondSetup()
        {
            kdfProvider.RunOneSecondSetup();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Iterations)));
        }

        public override TimeSpan RunTest()
        {
            return kdfProvider.RunTest();
        }
    }
}