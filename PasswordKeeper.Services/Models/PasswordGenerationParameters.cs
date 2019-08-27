namespace PasswordKeeper.Services.Models
{
    public class PasswordGenerationParameters
    {
        public int Length { get; set; }

        public bool IncludeSpecialCharacters { get; set; }

        public bool IncludeNumericCharacters { get; set; }

        public bool IncludeLowerCaseCharacters { get; set; }

        public bool IncludeUpperCaseCharacters { get; set; }

        public bool ExcludeSimilarCharacters { get; set; }

        public bool ExcludeAmbiguousCharacters { get; set; }
    }
}