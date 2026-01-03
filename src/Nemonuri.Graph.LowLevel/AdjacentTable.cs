
namespace Nemonuri.Graph.LowLevel;

public struct AdjacentTable<TEdgeLabel, TNodeKey, TReceiver> : 
    ILowLevelTable<TEdgeLabel, TNodeKey, MemoryViewReceiver<TReceiver, LowLevelKeyValuePair<TEdgeLabel, TNodeKey>>>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    private LowLevelTableReceiver<TReceiver, TEdgeLabel, TNodeKey> _table;

    public AdjacentTable(LowLevelTableReceiver<TReceiver, TEdgeLabel, TNodeKey> table)
    {
        _table = table;
    }

    [UnscopedRef]
    public ref readonly MemoryViewReceiver<TReceiver, LowLevelKeyValuePair<TEdgeLabel, TNodeKey>> InvokeProvider() => ref _table.InvokeProvider();
}


public readonly struct AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    public readonly AdjacentTable<TEdgeLabel, TNodeKey, TReceiver> AdjacentTable;
    public readonly TNodeValue NodeValue;

    public AdjacentTableAndValue(AdjacentTable<TEdgeLabel, TNodeKey, TReceiver> adjacents, TNodeValue nodeValue)
    {
        AdjacentTable = adjacents;
        NodeValue = nodeValue;
    }
}
