using System.Security.Cryptography;
using System.Text;
using ProfileApi.Models;

namespace ProfileApi.Services
{
    public class StringAnalyzerService
    {
        public AnalyzedString Analyze(string value)
        {
            var lower = value.ToLowerInvariant();
            return new AnalyzedString
            {
                Id = ComputeSha256(value),
                Value = value,
                Length = value.Length,
                IsPalindrome = lower.SequenceEqual(lower.Reverse()),
                UniqueCharacters = lower.Distinct().Count(),
                WordCount = string.IsNullOrWhiteSpace(value) ? 0 :
                             value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length,
                CharacterFrequencyMap = GetCharacterFrequency(value)
            };
        }

        private static string ComputeSha256(string raw)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(raw));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        private static Dictionary<char, int> GetCharacterFrequency(string input)
        {
            var dict = new Dictionary<char, int>();
            foreach (char c in input)
            {
                if (dict.ContainsKey(c)) dict[c]++;
                else dict[c] = 1;
            }
            return dict;
        }
    }
}
