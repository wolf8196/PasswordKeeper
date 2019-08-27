using System;
using System.Text;
using PasswordKeeper.Security;
using PasswordKeeper.Services.Abstractions;
using PasswordKeeper.Services.Models;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Services
{
    public class PasswordGenerationService : IGenerator<string, PasswordGenerationParameters>
    {
        private const string SpecialCharacters = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
        private const string LowerCaseCharacters = "abcdefghijklmnopqrstuvwxyz";
        private const string UpperCaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string NumericCharacters = "1234567890";
        private const string SimilarCharacters = "0oO1lLiI";
        private const string AmbiguousCharacters = "{}[]()/\'\"`~,;:.<>";

        private readonly IRandomNumberGenerator generator;

        public PasswordGenerationService(IRandomNumberGenerator generator)
        {
            this.generator = generator;
        }

        public string Generate(PasswordGenerationParameters parameters)
        {
            parameters.ThrowIfNull(nameof(parameters));

            // Create character set based on parameters
            var charSet = new StringBuilder();
            var password = new StringBuilder(parameters.Length);

            IncludeCharacters(charSet, SpecialCharacters, parameters.IncludeSpecialCharacters);
            IncludeCharacters(charSet, LowerCaseCharacters, parameters.IncludeLowerCaseCharacters);
            IncludeCharacters(charSet, UpperCaseCharacters, parameters.IncludeUpperCaseCharacters);
            IncludeCharacters(charSet, NumericCharacters, parameters.IncludeNumericCharacters);
            ExcludeCharacters(charSet, SimilarCharacters, parameters.ExcludeSimilarCharacters);
            ExcludeCharacters(charSet, AmbiguousCharacters, parameters.ExcludeAmbiguousCharacters);

            var length = parameters.Length;

            // Randomly choose a letter from character set & append it to the password
            while (length-- > 0)
            {
                password.Append(charSet[Math.Abs(generator.GetInt32()) % charSet.Length]);
            }

            return password.ToString();
        }

        private static void IncludeCharacters(StringBuilder target, string characters, bool flag)
        {
            if (flag)
            {
                target.Append(characters);
            }
        }

        private static void ExcludeCharacters(StringBuilder target, string characters, bool flag)
        {
            if (flag)
            {
                foreach (var character in characters)
                {
                    target.Replace(character.ToString(), string.Empty);
                }
            }
        }
    }
}