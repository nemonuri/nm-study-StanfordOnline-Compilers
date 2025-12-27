
namespace Nemonuri.LowLevel;

public readonly struct PackedGraphView<TNodeKey, TEdgeLabel, TNodeValue, TConfig> :
    IMemoryView<LowLevelKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TConfig>>>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    private readonly PackedTableView<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TConfig>> _table;

    public PackedGraphView(PackedTableView<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TConfig>> table)
    {
        _table = table;
    }

    public int Length => _table.Length;

    public ref LowLevelKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TConfig>> this[int index] => ref _table[index];
}


public readonly struct PackedGraph<TNodeKey, TEdgeLabel, TNodeValue, TConfig> :
    ILowLevelGraph<TNodeKey, TEdgeLabel, TNodeValue, PackedGraphView<TNodeKey, TEdgeLabel, TNodeValue, TConfig>, TConfig>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    private readonly PackedTable<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TConfig>> _packedTable;

    public PackedGraph(LowLevelKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TConfig>>[] memory)
    {
        _packedTable = new(memory);
    }

    public LowLevelKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TConfig>>[] Memory => _packedTable.Memory;

    public void GetMemoryView(scoped ref PackedGraphView<TNodeKey, TEdgeLabel, TNodeValue, TConfig> memoryView)
    {
        PackedTableView<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TConfig>> innerMemoryView = default;
        _packedTable.GetMemoryView(ref innerMemoryView);
        memoryView = new(innerMemoryView);
    }
}

