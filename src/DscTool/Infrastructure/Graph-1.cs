
using System.Collections;

namespace DscTool.Infrastructure;

public class Graph<T> : IReadOnlyDictionary<T, ReadOnlyMemory<T>>
    where T : IEquatable<T>
{
    private readonly Dictionary<T, ReadOnlyMemory<T>> _table;

    public Graph()
    {
        _table = new();
    }

    public void Add(T node, ReadOnlyMemory<T> dependants)
    {
        _table.Add(node, dependants);
    }

    public bool ContainsKey(T key)
    {
        return _table.ContainsKey(key);
    }

    public bool TryGetValue(T key, out ReadOnlyMemory<T> value)
    {
        return _table.TryGetValue(key, out value);
    }

    public ReadOnlyMemory<T> this[T key] => _table[key];

    public IEnumerable<T> Keys => _table.Keys;

    public IEnumerable<ReadOnlyMemory<T>> Values => _table.Values;

    public int Count => _table.Count;

    public IEnumerator<KeyValuePair<T, ReadOnlyMemory<T>>> GetEnumerator()
    {
        return _table.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
