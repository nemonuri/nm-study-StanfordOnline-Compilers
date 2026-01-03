
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
    private readonly PackedTable<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>> _packedTable;

    public PackedGraph(LowLevelKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>[] memory)
    {
        _packedTable = new(memory);
    }

    public LowLevelKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>[] Memory => _packedTable.Memory;

    public void GetMemoryView(scoped ref PackedGraphView<TNodeKey, TEdgeLabel, TNodeValue, TReceiver> memoryView)
    {
        PackedTableView<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>> innerMemoryView = default;
        _packedTable.GetMemoryView(ref innerMemoryView);
        memoryView = new(innerMemoryView);
    }
}

