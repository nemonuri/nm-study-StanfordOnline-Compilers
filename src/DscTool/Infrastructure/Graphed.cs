namespace DscTool.Infrastructure;

public readonly record struct Graphed<T>(Graph<T> Graph, T Node)
    where T : IEquatable<T>;

public readonly record struct Graphed<TKey, TValue>(Graph<TKey, TValue> Graph, KeyValuePair<TKey, TValue> Entry)
    where TKey : IEquatable<TKey>;

public readonly record struct Graphed<TNodeKey, TEdgeKey, TValue>
(
    Graph<TNodeKey, TEdgeKey, TValue> Graph, 
    KeyValuePair<TNodeKey, TValue> Entry
)
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeKey : IEquatable<TEdgeKey>
    ;
