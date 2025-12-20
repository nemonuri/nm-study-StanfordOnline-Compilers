using System.Collections;

namespace DscTool.Scoped.Hashtables;

public readonly partial struct ScopedDictionaryCategoriedCommand<T, TCondition, TCategory, TCategoriedCommand, TKey> where TCategory : IScopedCategory<T, TCondition>
    where TCategoriedCommand : IScopedCategoriedCommand<T, TCondition, TCategory>
    where TKey : IEquatable<TKey>
{
    private class CategoryTable : IReadOnlyDictionary<TKey, TCategory>
    {
        private readonly IReadOnlyDictionary<TKey, TCategoriedCommand> _commandTable;

        public CategoryTable(IReadOnlyDictionary<TKey, TCategoriedCommand> commandTable)
        {
            _commandTable = commandTable;
        }

        public bool ContainsKey(TKey key) => _commandTable.ContainsKey(key);

        public bool TryGetValue(TKey key, out TCategory value)
        {
            if (_commandTable.TryGetValue(key, out var command))
            {
                value = command.Category;
                return true;
            }
            else
            {
                value = default!;
                return false;
            }
        }

        public TCategory this[TKey key] => _commandTable[key].Category;

        public IEnumerable<TKey> Keys => _commandTable.Keys;

        public IEnumerable<TCategory> Values => _commandTable.Values.Select(static c => c.Category);

        public int Count => _commandTable.Count;

        public IEnumerator<KeyValuePair<TKey, TCategory>> GetEnumerator() =>
            _commandTable.Select(static pair => new KeyValuePair<TKey, TCategory>(pair.Key, pair.Value.Category)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
