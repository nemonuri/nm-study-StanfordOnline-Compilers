
namespace Nemonuri.LowLevel;

public readonly struct MemoryAndSelector<T, TView>(Memory<T> memory, RefSelectorHandle<T, TView> selector)
{
    public readonly Memory<T> Memory = memory;
    public readonly RefSelectorHandle<T, TView> Selector = selector;
}

public readonly ref struct SpanAndSelector<T, TView>(Span<T> span, RefSelectorHandle<T, TView> selector)
{
    public readonly Span<T> Span = span;
    public readonly RefSelectorHandle<T, TView> Selector = selector;
}

public struct AdjacentTable<TEdgeLabel, TNodeKey, TConfig> : 
    ILowLevelTable<TEdgeLabel, TNodeKey, AbstractMemoryView<TConfig, LowLevelKeyValuePair<TEdgeLabel, TNodeKey>>>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    private AbstractLowLevelTable<TConfig, TEdgeLabel, TNodeKey> _table;

    public AdjacentTable(AbstractLowLevelTable<TConfig, TEdgeLabel, TNodeKey> table)
    {
        _table = table;
    }

    public void GetMemoryView(scoped ref AbstractMemoryView<TConfig, LowLevelKeyValuePair<TEdgeLabel, TNodeKey>> memoryView) =>
        _table.GetMemoryView(ref memoryView);
}

public readonly struct AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TConfig>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    public readonly AdjacentTable<TEdgeLabel, TNodeKey, TConfig> AdjacentTable;
    public readonly TNodeValue NodeValue;

    public AdjacentTableAndValue(AdjacentTable<TEdgeLabel, TNodeKey, TConfig> adjacents, TNodeValue nodeValue)
    {
        AdjacentTable = adjacents;
        NodeValue = nodeValue;
    }
}
