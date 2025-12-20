
using System.Collections;

namespace DscTool.Scoped.Hashtables;

public readonly partial struct ScopedDictionaryHoareTripleCommand<T, TCondition, TCommand, TKey> where TKey : IEquatable<TKey>
    where TCommand : IScopedHoareTripleCommand<T, TCondition>
{
    private class PreConditionTable : IReadOnlyDictionary<TKey, TCondition>
    {
        private readonly IReadOnlyDictionary<TKey, TCommand> _commandTable;

        public PreConditionTable(IReadOnlyDictionary<TKey, TCommand> commandTable)
        {
            _commandTable = commandTable;
        }

        public bool ContainsKey(TKey key) => _commandTable.ContainsKey(key);

        public bool TryGetValue(TKey key, out TCondition value)
        {
            if (_commandTable.TryGetValue(key, out var command))
            {
                value = command.PreCondition;
                return true;
            }
            else
            {
                value = default!;
                return false;
            }
        }

        public TCondition this[TKey key] => _commandTable[key].PreCondition;

        public IEnumerable<TKey> Keys => _commandTable.Keys;

        public IEnumerable<TCondition> Values => _commandTable.Values.Select(static c => c.PreCondition);

        public int Count => _commandTable.Count;

        public IEnumerator<KeyValuePair<TKey, TCondition>> GetEnumerator() =>
            _commandTable.Select(static pair => new KeyValuePair<TKey, TCondition>(pair.Key, pair.Value.PreCondition)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
