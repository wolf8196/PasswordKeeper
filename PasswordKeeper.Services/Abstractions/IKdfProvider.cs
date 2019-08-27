using System;
using PasswordKeeper.Security;

namespace PasswordKeeper.Services.Abstractions
{
    public interface IKdfProvider
    {
        IKeyDerivationFunction GetKeyDerivationFunction();

        void RunOneSecondSetup();

        TimeSpan RunTest();
    }
}