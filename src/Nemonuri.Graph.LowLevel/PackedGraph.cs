
namespace Nemonuri.Graph.LowLevel;

public readonly struct PackedGraphView<TNodeKey, TEdgeLabel, TNodeValue, TReceiver> :
    IMemoryView<LowLevelKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>>,
    IMaybeSupportsRawSpan<LowLevelKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    private readonly PackedTableView<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>> _table;

    public PackedGraphView(PackedTableView<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>> table)
    {
        _table = table;
    }

    public int Length => _table.Length;

    public ref LowLevelKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>> this[int index] => ref _table[index];

    public bool SupportsRawSpan => _table.SupportsRawSpan;

    public Span<LowLevelKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>> AsSpan => _table.AsSpan;
}


public readonly partial struct PackedGraph<TNodeKey, TEdgeLabel, TNodeValue, TReceiver> :
    ILowLevelGraph<TNodeKey, TEdgeLabel, TNodeValue, PackedGraphView<TNodeKey, TEdgeLabel, TNodeValue, TReceiver>, TReceiver>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    private readonly PackedGraphView<TNodeKey, TEdgeLabel, TNodeValue, TReceiver> _packedGraphView;

    public PackedGraph(PackedGraphView<TNodeKey, TEdgeLabel, TNodeValue, TReceiver> packedGraphView)
    {
        _packedGraphView = packedGraphView;
    }

    public PackedGraph(LowLevelKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>[] memory) :
    this
    (
        new PackedGraphView<TNodeKey, TEdgeLabel, TNodeValue, TReceiver>
        (
            new PackedTableView<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>
            (
                new ArrayView<LowLevelKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>>(memory)
            )
        )
    )
    { }

    [UnscopedRef]
    public ref readonly PackedGraphView<TNodeKey, TEdgeLabel, TNodeValue, TReceiver> InvokeProvider() => ref _packedGraphView;
}
