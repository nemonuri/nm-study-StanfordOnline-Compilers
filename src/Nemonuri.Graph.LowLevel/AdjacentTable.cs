
using Nemonuri.LowLevel.DuckTyping;

namespace Nemonuri.Graph.LowLevel;

public readonly struct AdjacentTableView<TEdgeLabel, TNodeKey, TReceiver> :
    IMemoryView<LowLevelKeyValuePair<TEdgeLabel, TNodeKey>>, 
    IMaybeSupportsRawSpan<LowLevelKeyValuePair<TEdgeLabel, TNodeKey>>, 
    IDuckTypeReceiver<TReceiver>,
    IAddable<LowLevelKeyValuePair<TEdgeLabel, TNodeKey>>
{
    private readonly
    DuckTypedDynamicMemoryView<TReceiver, LowLevelKeyValuePair<TEdgeLabel, TNodeKey>, DuckTypedMemoryView<TReceiver, LowLevelKeyValuePair<TEdgeLabel, TNodeKey>>, TNodeKey>
    _memoryView;

    internal AdjacentTableView(DuckTypedDynamicMemoryView<TReceiver, LowLevelKeyValuePair<TEdgeLabel, TNodeKey>, DuckTypedMemoryView<TReceiver, LowLevelKeyValuePair<TEdgeLabel, TNodeKey>>, TNodeKey> memoryView)
    {
        _memoryView = memoryView;
    }

    public void Add(in LowLevelKeyValuePair<TEdgeLabel, TNodeKey> source) => _memoryView.Add(in source);

    [UnscopedRef]
    public ref readonly TReceiver Receiver => ref Unsafe.AsRef(in _memoryView).Receiver;

    public bool SupportsRawSpan => _memoryView.SupportsRawSpan;

    [UnscopedRef]
    public Span<LowLevelKeyValuePair<TEdgeLabel, TNodeKey>> AsSpan => Unsafe.AsRef(in _memoryView).AsSpan;

    public int Length => _memoryView.Length;

    public ref LowLevelKeyValuePair<TEdgeLabel, TNodeKey> this[int index] => ref Unsafe.AsRef(in _memoryView)[index];
}

public struct AdjacentTable<TEdgeLabel, TNodeKey, TReceiver> : 
    ILowLevelTable<TEdgeLabel, TNodeKey, AdjacentTableView<TEdgeLabel, TNodeKey, TReceiver>>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    private TNodeKey _nodeKey;
    private readonly DuckTypedMethod<TReceiver, TNodeKey, AdjacentTableView<TEdgeLabel, TNodeKey, TReceiver>> _memoryViewProvider;

    public AdjacentTable
    (
        TNodeKey nodeKey,
        DuckTypedMethod<TReceiver, TNodeKey, AdjacentTableView<TEdgeLabel, TNodeKey, TReceiver>> memoryViewProvider
    )
    {
        _nodeKey = nodeKey;
        _memoryViewProvider = memoryViewProvider;
    }

    [UnscopedRef]
    public ref readonly AdjacentTableView<TEdgeLabel, TNodeKey, TReceiver> InvokeProvider() => ref Unsafe.AsRef(in _memoryViewProvider).InvokeMethod(ref _nodeKey!);
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
