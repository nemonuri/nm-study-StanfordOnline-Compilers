
using System.Collections;

namespace DscTool.Infrastructure;


public class Graph<TNodeKey, TEdgeKey, TValue> : IReadOnlyDictionary<TNodeKey, ReadOnlyMemory<KeyValuePair<TEdgeKey, KeyValuePair<TNodeKey, TValue>>>>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeKey : IEquatable<TEdgeKey>
{
    private readonly Dictionary<TNodeKey, ReadOnlyMemory<KeyValuePair<TEdgeKey, KeyValuePair<TNodeKey, TValue>>>> _table;

    public Graph()
    {
        _table = new();
    }

    public void Add(TNodeKey node, ReadOnlyMemory<KeyValuePair<TEdgeKey, KeyValuePair<TNodeKey, TValue>>> dependants)
    {
        _table.Add(node, dependants);
    }

    public bool ContainsKey(TNodeKey key)
    {
        return _table.ContainsKey(key);
    }

    public bool TryGetValue(TNodeKey key, out ReadOnlyMemory<KeyValuePair<TEdgeKey, KeyValuePair<TNodeKey, TValue>>> value)
    {
        return _table.TryGetValue(key, out value);
    }

    public ReadOnlyMemory<KeyValuePair<TEdgeKey, KeyValuePair<TNodeKey, TValue>>> this[TNodeKey key] => _table[key];

    public IEnumerable<TNodeKey> Keys => _table.Keys;

    public IEnumerable<ReadOnlyMemory<KeyValuePair<TEdgeKey, KeyValuePair<TNodeKey, TValue>>>> Values => _table.Values;

    public int Count => _table.Count;

    public IEnumerator<KeyValuePair<TNodeKey, ReadOnlyMemory<KeyValuePair<TEdgeKey, KeyValuePair<TNodeKey, TValue>>>>> GetEnumerator()
    {
        return _table.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
