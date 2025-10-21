using Microsoft.AspNetCore.Mvc;
using ProfileApi.Services;
using ProfileApi.Models;
using System.Text.Json;

namespace ProfileApi.Controllers
{
    [ApiController]
    [Route("strings")]
    public class StringsController : ControllerBase
    {
        private readonly StringRepository _repo;
        private readonly StringAnalyzerService _analyzer;

        public StringsController(StringRepository repo, StringAnalyzerService analyzer)
        {
            _repo = repo;
            _analyzer = analyzer;
        }

        // 1️⃣ POST /strings
        [HttpPost]
        public IActionResult AnalyzeString([FromBody] JsonElement body)
        {
            string? value = null;

            if (body.ValueKind == JsonValueKind.Object)
            {
                if (body.TryGetProperty("value", out JsonElement valueElement) && valueElement.ValueKind == JsonValueKind.String)
                    value = valueElement.GetString();
            }
            else if (body.ValueKind == JsonValueKind.String)
            {
                value = body.GetString();
            }

            if (string.IsNullOrWhiteSpace(value))
                return BadRequest(new { error = "Missing or invalid 'value' field" });

            if (_repo.Exists(value))
                return Conflict(new { error = "String already exists in the system" });

            var analyzed = _analyzer.Analyze(value);
            analyzed.CreatedAt = DateTime.UtcNow;

            _repo.Add(analyzed);

            return Created("", new
            {
                id = analyzed.Id,
                value = analyzed.Value,
                properties = new
                {
                    analyzed.Length,
                    analyzed.IsPalindrome,
                    analyzed.UniqueCharacters,
                    analyzed.WordCount,
                    sha256_hash = analyzed.Id,
                    analyzed.CharacterFrequencyMap
                },
                created_at = analyzed.CreatedAt
            });
        }



        // 2️⃣ GET /strings/{string_value}
        [HttpGet("{string_value}")]
        public IActionResult GetString(string string_value)
        {
            var item = _repo.Get(string_value);
            if (item == null) return NotFound(new { error = "String not found" });

            return Ok(new
            {
                id = item.Id,
                value = item.Value,
                properties = new
                {
                    item.Length,
                    item.IsPalindrome,
                    item.UniqueCharacters,
                    item.WordCount,
                    item.Id,
                    item.CharacterFrequencyMap
                },
                created_at = item.CreatedAt
            });
        }

        // 3️⃣ GET /strings with filters
        [HttpGet]
        public IActionResult GetAll([FromQuery] bool? is_palindrome, [FromQuery] int? min_length,
            [FromQuery] int? max_length, [FromQuery] int? word_count, [FromQuery] string? contains_character)
        {
            var list = _repo.GetAll().AsQueryable();

            if (is_palindrome.HasValue)
                list = list.Where(s => s.IsPalindrome == is_palindrome.Value);
            if (min_length.HasValue)
                list = list.Where(s => s.Length >= min_length.Value);
            if (max_length.HasValue)
                list = list.Where(s => s.Length <= max_length.Value);
            if (word_count.HasValue)
                list = list.Where(s => s.WordCount == word_count.Value);
            if (!string.IsNullOrEmpty(contains_character))
                list = list.Where(s => s.Value.Contains(contains_character, StringComparison.OrdinalIgnoreCase));

            var data = list.ToList();

            return Ok(new
            {
                data,
                count = data.Count,
                filters_applied = new
                {
                    is_palindrome,
                    min_length,
                    max_length,
                    word_count,
                    contains_character
                }
            });
        }

        // 4️⃣ GET /strings/filter-by-natural-language
        [HttpGet("filter-by-natural-language")]
        public IActionResult FilterByNaturalLanguage([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { error = "Missing query" });

            query = query.ToLowerInvariant();

            bool? isPalindrome = null;
            int? wordCount = null;
            int? minLength = null;
            string? containsCharacter = null;

            if (query.Contains("palindromic"))
                isPalindrome = true;
            if (query.Contains("single word"))
                wordCount = 1;
            if (query.Contains("longer than"))
            {
                var parts = query.Split("longer than");
                if (int.TryParse(new string(parts.Last().Where(char.IsDigit).ToArray()), out int len))
                    minLength = len + 1;
            }
            if (query.Contains("letter"))
            {
                var c = query.Last();
                if (char.IsLetter(c))
                    containsCharacter = c.ToString();
            }

            if (isPalindrome == null && wordCount == null && minLength == null && containsCharacter == null)
                return BadRequest(new { error = "Unable to parse natural language query" });

            var list = _repo.GetAll().AsQueryable();
            if (isPalindrome.HasValue)
                list = list.Where(s => s.IsPalindrome == isPalindrome.Value);
            if (wordCount.HasValue)
                list = list.Where(s => s.WordCount == wordCount.Value);
            if (minLength.HasValue)
                list = list.Where(s => s.Length >= minLength.Value);
            if (!string.IsNullOrEmpty(containsCharacter))
                list = list.Where(s => s.Value.Contains(containsCharacter, StringComparison.OrdinalIgnoreCase));

            var data = list.ToList();

            return Ok(new
            {
                data,
                count = data.Count,
                interpreted_query = new
                {
                    original = query,
                    parsed_filters = new
                    {
                        word_count = wordCount,
                        is_palindrome = isPalindrome,
                        min_length = minLength,
                        contains_character = containsCharacter
                    }
                }
            });
        }

        // 5️⃣ DELETE /strings/{string_value}
        [HttpDelete("{string_value}")]
        public IActionResult DeleteString(string string_value)
        {
            var removed = _repo.Remove(string_value);
            if (!removed) return NotFound(new { error = "String not found" });
            return NoContent();
        }
    }
}
