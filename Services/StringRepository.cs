using ProfileApi.Models;

namespace ProfileApi.Services
{
    public class StringRepository
    {
        private readonly Dictionary<string, AnalyzedString> _store = new();

        public bool Exists(string value)
        {
            return _store.Values.Any(s => s.Value.Equals(value, StringComparison.OrdinalIgnoreCase));
        }

        public void Add(AnalyzedString analyzed)
        {
            _store[analyzed.Id] = analyzed;
        }

        public AnalyzedString? Get(string value)
        {
            return _store.Values.FirstOrDefault(s => s.Value.Equals(value, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<AnalyzedString> GetAll() => _store.Values;

        public bool Remove(string value)
        {
            var item = Get(value);
            if (item == null) return false;
            return _store.Remove(item.Id);
        }
    }
}
