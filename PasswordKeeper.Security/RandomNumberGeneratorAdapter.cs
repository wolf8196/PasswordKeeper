using System;
using System.Security.Cryptography;

namespace PasswordKeeper.Security
{
    public class RandomNumberGeneratorAdapter : IRandomNumberGenerator
    {
        private static readonly RandomNumberGenerator Generator = RandomNumberGenerator.Create();

        public int GetInt32()
        {
            var bytes = new byte[sizeof(int)];
            Generator.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}