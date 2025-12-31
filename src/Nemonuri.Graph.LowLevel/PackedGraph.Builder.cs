
namespace Nemonuri.Graph.LowLevel;

public readonly partial struct PackedGraph<TNodeKey, TEdgeLabel, TNodeValue, TReceiver> where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    public static Builder CreateBuilder(int initialCapacity = 4) => new(initialCapacity);

    public readonly struct Builder :
        IMemoryView<LowLevelKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>>,
        IMaybeSupportsRawSpan<LowLevelKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>>
    {
        private readonly PackedTable<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>.Builder _builder;

        internal Builder (int initialCapacity)
        {
            _builder = new(initialCapacity);
        }

        public int Length => _builder.Length;

        public ref LowLevelKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>> this[int index] => ref _builder[index];

        public bool SupportsRawSpan => _builder.SupportsRawSpan;

        public Span<LowLevelKeyValuePair<TNodeKey, AdjacentTableAndValue<TEdgeLabel, TNodeKey, TNodeValue, TReceiver>>> AsSpan => _builder.AsSpan;

        public void AddKey(TNodeKey key)
        {
            //_builder.Add(key, new ())
        }
    }
}

