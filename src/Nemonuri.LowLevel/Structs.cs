
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

public readonly struct LabeledKey<TLabel, TKey>
    where TLabel : IEquatable<TLabel>
    where TKey : IEquatable<TKey>
{
    public readonly TLabel Label;
    public readonly TKey Key;

    public LabeledKey(TLabel label, TKey key)
    {
        Label = label;
        Key = key;
    }
}

public readonly struct AdjacentsAndValue<TEdgeLabel, TNodeKey, TAdjacentsHandler, TNodeValue>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    public readonly AbstractMemoryView<TAdjacentsHandler, LabeledKey<TEdgeLabel, TNodeKey>> Adjacents;
    public readonly TNodeValue NodeValue;

    public AdjacentsAndValue(AbstractMemoryView<TAdjacentsHandler, LabeledKey<TEdgeLabel, TNodeKey>> adjacents, TNodeValue nodeValue)
    {
        Adjacents = adjacents;
        NodeValue = nodeValue;
    }
}
