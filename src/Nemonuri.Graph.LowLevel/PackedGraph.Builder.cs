
using Nemonuri.LowLevel.Abstractions;
using Nemonuri.LowLevel.DuckTyping;

namespace Nemonuri.Graph.LowLevel;

public readonly partial struct PackedGraph<TNodeKey, TEdgeLabel, TNodeValue, TReceiver> 
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    public struct Builder
    {
        private readonly DangerousMemoryViewProviderBuilder<ObjectOrPointer, PackedGraphMemoryKey<TNodeKey, TEdgeLabel>> _builder;

        internal Builder
        (
            DuckTypedProvider<TReceiver, DangerousMemoryViewProviderBuilder<ObjectOrPointer, PackedGraphMemoryKey<TNodeKey, TEdgeLabel>>> builderProviderHandle
        )
        {
            var b = builderProviderHandle.InvokeProvider();
            Guard.IsNotNull(b, "builderProviderHandle.InvokeProvider()");
            _builder = b;
        }

        public int GetOrAddNodeKey(TNodeKey nodeKey, out bool fresh)
        {
            PackedGraphMemoryKey<TNodeKey, TEdgeLabel> mk = new(nodeKey);
            return _builder.GetOrAddArgumentComponent(mk, out fresh);
        }
    }
}
