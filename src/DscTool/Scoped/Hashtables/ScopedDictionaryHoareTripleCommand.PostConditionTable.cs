using System.Collections;

namespace DscTool.Scoped.Hashtables;

public readonly partial struct ScopedDictionaryHoareTripleCommand<T, TCondition, TCommand, TKey> where TKey : IEquatable<TKey>
    where TCommand : IScopedHoareTripleCommand<T, TCondition>
{
    private class PostConditionTable : IReadOnlyDictionary<TKey, TCondition>
    {
        private readonly Dictionary<TKey, TCondition> _conditionTable;

        public PostConditionTable(IReadOnlyCollection<KeyValuePair<TKey, TCondition>> entries)
        {
            _conditionTable = new(capacity: entries.Count);
            foreach (var entry in entries)
            {
                _conditionTable.Add(entry.Key, entry.Value);
            }
        }

        public bool ContainsKey(TKey key)
        {
            return _conditionTable.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TCondition value)
        {
            return _conditionTable.TryGetValue(key, out value);
        }

        public TCondition this[TKey key] => _conditionTable[key];

        public IEnumerable<TKey> Keys => _conditionTable.Keys;

        public IEnumerable<TCondition> Values => _conditionTable.Values;

        public int Count => _conditionTable.Count;

        public IEnumerator<KeyValuePair<TKey, TCondition>> GetEnumerator()
        {
            return _conditionTable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _conditionTable.GetEnumerator();
        }
    }
}
