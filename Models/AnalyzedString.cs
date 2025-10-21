namespace ProfileApi.Models
{
    public class AnalyzedString
    {
        public string Id { get; set; } = string.Empty; // SHA-256 hash
        public string Value { get; set; } = string.Empty;
        public int Length { get; set; }
        public bool IsPalindrome { get; set; }
        public int UniqueCharacters { get; set; }
        public int WordCount { get; set; }
        public Dictionary<char, int> CharacterFrequencyMap { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
