namespace DscTool.Infrastructure;

public readonly record struct OptionalGraphed<TNodeKey, TValue>
(
    Graph<TNodeKey, TValue>? OptionalGraph, 
    KeyValuePair<OptionalKey<TNodeKey>, TValue> OptionalKeyedEntry
)
    where TNodeKey : IEquatable<TNodeKey>
{
    public bool TryGetNodeKey([NotNullWhen(true)] out TNodeKey? nodeKey)
    {
        var key = OptionalKeyedEntry.Key;
        if (!key.IsSome) {nodeKey = default; return false;}

        nodeKey = key.GetSome();
        return true;
    }
}

public readonly record struct OptionalGraphed<TNodeKey, TEdgeKey, TValue>
(
    Graph<TNodeKey, TEdgeKey, TValue>? OptionalGraph, 
    KeyValuePair<OptionalKey<TNodeKey>, TValue> OptionalKeyedEntry
)
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeKey : IEquatable<TEdgeKey>
{
    public bool TryGetNodeKey([NotNullWhen(true)] out TNodeKey? nodeKey)
    {
        var key = OptionalKeyedEntry.Key;
        if (!key.IsSome) {nodeKey = default; return false;}

        nodeKey = key.GetSome();
        return true;
    }
}
;
