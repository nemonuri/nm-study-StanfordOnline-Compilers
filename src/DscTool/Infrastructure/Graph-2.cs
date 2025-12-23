
using System.Collections;

namespace DscTool.Infrastructure;


public class Graph<TKey, TValue> : IReadOnlyDictionary<TKey, ReadOnlyMemory<KeyValuePair<TKey, TValue>>>
    where TKey : IEquatable<TKey>
{
    private readonly Dictionary<TKey, ReadOnlyMemory<KeyValuePair<TKey, TValue>>> _table;

    public Graph()
    {
        _table = new();
    }

    public void Add(TKey node, ReadOnlyMemory<KeyValuePair<TKey, TValue>> dependants)
    {
        _table.Add(node, dependants);
    }

    public bool ContainsKey(TKey key)
    {
        return _table.ContainsKey(key);
    }

    public bool TryGetValue(TKey key, out ReadOnlyMemory<KeyValuePair<TKey, TValue>> value)
    {
        return _table.TryGetValue(key, out value);
    }

    public ReadOnlyMemory<KeyValuePair<TKey, TValue>> this[TKey key] => _table[key];

    public IEnumerable<TKey> Keys => _table.Keys;

    public IEnumerable<ReadOnlyMemory<KeyValuePair<TKey, TValue>>> Values => _table.Values;

    public int Count => _table.Count;

    public IEnumerator<KeyValuePair<TKey, ReadOnlyMemory<KeyValuePair<TKey, TValue>>>> GetEnumerator()
    {
        return _table.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
