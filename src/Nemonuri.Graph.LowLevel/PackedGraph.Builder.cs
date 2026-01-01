
namespace Nemonuri.Graph.LowLevel;

public readonly partial struct PackedGraph<TNodeKey, TEdgeLabel, TNodeValue, TReceiver> where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    public static Builder<TReceiverRefBox> CreateBuilder<TReceiverRefBox>
    (
        in TReceiverRefBox refBox, 
        BuilderHandle<
            TReceiver, 
            RawKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>,
            PackedGraphView<TNodeKey, TEdgeLabel, TNodeValue, TReceiver>> 
            graphBuilderHandle,
        BuilderHandle<
            TReceiver,
            RawKeyValuePair<TEdgeLabel, TNodeKey>,
            MemoryViewReceiver<TReceiver, RawKeyValuePair<TEdgeLabel, TNodeKey>>>
            adjacentTableBuilderHandle,
        int initialCapacity = 4
    ) 
        where TReceiverRefBox : IRefBox<TReceiver>
        => new(in refBox, graphBuilderHandle, adjacentTableBuilderHandle, initialCapacity);

    public struct Builder<TReceiverRefBox> :
        IMemoryView<RawKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>>,
        IMaybeSupportsRawSpan<RawKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>>
        where TReceiverRefBox : IRefBox<TReceiver>
    {
        private readonly TReceiverRefBox _refBox;

        private readonly 
        BuilderHandle<
            TReceiver, 
            RawKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>,
            PackedGraphView<TNodeKey, TEdgeLabel, TNodeValue, TReceiver>> 
            _graphBuilderHandle;

        private readonly 
        BuilderHandle<
            TReceiver,
            RawKeyValuePair<TEdgeLabel, TNodeKey>,
            MemoryViewReceiver<TReceiver, RawKeyValuePair<TEdgeLabel, TNodeKey>>>
            _adjacentTableBuilderHandle;

        private readonly PackedTable<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>.Builder _builder;

        internal Builder
        (
            in TReceiverRefBox refBox, 
            BuilderHandle<
                TReceiver, 
                RawKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>,
                PackedGraphView<TNodeKey, TEdgeLabel, TNodeValue, TReceiver>> 
                graphBuilderHandle,
            BuilderHandle<
                TReceiver,
                RawKeyValuePair<TEdgeLabel, TNodeKey>,
                MemoryViewReceiver<TReceiver, RawKeyValuePair<TEdgeLabel, TNodeKey>>>
                adjacentTableBuilderHandle,
            int initialCapacity = 4
        ) 
        {
            _refBox = refBox;
            _graphBuilderHandle = graphBuilderHandle;
            _adjacentTableBuilderHandle = adjacentTableBuilderHandle;
            _builder = new(initialCapacity);
        }

        public int Length => _builder.Length;

        public ref RawKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>> this[int index] => ref _builder[index];

        public bool SupportsRawSpan => _builder.SupportsRawSpan;

        public Span<RawKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>> AsSpan => _builder.AsSpan;

        public void AddKey(TNodeKey key)
        {
            //_builder.Add(key, new ())
        }
    }
}

