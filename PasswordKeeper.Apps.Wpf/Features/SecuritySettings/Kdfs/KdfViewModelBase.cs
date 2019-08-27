using System;
using PasswordKeeper.Services.Abstractions;

namespace PasswordKeeper.Apps.Wpf.Features.SecuritySettings.Kdfs
{
    public abstract class KdfViewModelBase : ViewModelBase
    {
        public abstract IKdfProvider GetKdfProvider();

        public abstract void RunOneSecondSetup();

        public abstract TimeSpan RunTest();
    }
}