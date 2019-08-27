using System;
using System.Diagnostics;
using System.Text;
using PasswordKeeper.Security;
using PasswordKeeper.Services.Abstractions;

namespace PasswordKeeper.Services.CryptoProviders
{
    public class Pbkdf2Provider : IKdfProvider
    {
        private const int TestKeySize = 128;
        private const string TestPassword = "testPassword123";
        private const int TestIterations = 20000;

        public Pbkdf2Provider()
        {
            Iterations = 100000;
        }

        public int Iterations { get; set; }

        public IKeyDerivationFunction GetKeyDerivationFunction()
        {
            return new Pbkdf2Sha512(8, Iterations);
        }

        public void RunOneSecondSetup()
        {
            var watch = new Stopwatch();
            watch.Start();
            new Pbkdf2Sha512(8, TestIterations).Generate(Encoding.UTF8.GetBytes(TestPassword), TestKeySize);
            watch.Stop();

            Iterations = Convert.ToInt32(TestIterations / watch.Elapsed.TotalSeconds);
        }

        public TimeSpan RunTest()
        {
            var watch = new Stopwatch();
            watch.Start();

            new Pbkdf2Sha512(8, Iterations).Generate(Encoding.UTF8.GetBytes(TestPassword), TestKeySize);

            watch.Stop();
            return watch.Elapsed;
        }
    }
}