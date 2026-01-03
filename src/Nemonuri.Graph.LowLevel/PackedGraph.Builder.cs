
using Nemonuri.LowLevel.Abstractions;
using Nemonuri.LowLevel.DuckTyping;

namespace Nemonuri.Graph.LowLevel;

public readonly partial struct PackedGraph<TNodeKey, TEdgeLabel, TNodeValue, TReceiver> 
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    public struct Builder
    {
        private readonly TReceiver _receiver;
        private readonly DangerousMemoryViewProviderBuilder<Reference, nint> _builder;
        private readonly FactoryHandle<TReceiver, PackedGraphMemoryKey<TNodeKey>, TypedMethodCallEntry> _factoryHandle;
        private DangerousMemoryViewProviderBuilder<Reference, nint>.Provider _rootTableProvider;

        internal Builder
        (
            TReceiver receiver,
            ProviderHandle<TReceiver, DangerousMemoryViewProviderBuilder<Reference, nint>> providerBuilderHandle,
            FactoryHandle<TReceiver, PackedGraphMemoryKey<TNodeKey>, TypedMethodCallEntry> factoryHandle
        )
        {
            _receiver = receiver;
            var b = providerBuilderHandle.InvokeProvider(in receiver);
            Guard.IsNotNull(b, "builderProviderHandle.InvokeProvider()");
            _builder = b;
            _factoryHandle = factoryHandle;
        }

        public LowLevelKeyValuePair<int, DangerousMemoryViewProviderBuilder<Reference, nint>.Provider> 
        GetOrAddMemoryKey(PackedGraphMemoryKey<TNodeKey> memoryKey, out DangerousMemoryViewProviderBuilderTheory.AddAndBuildFresh fresh)
        {
            var entry = _factoryHandle.InvokeFactory(in _receiver, memoryKey);
            return
                _builder.AddAndBuild(entry.MethodCallEntry.Receiver, entry.MethodCallEntry.Argument, new TypedUnmanagedBox<nint>(entry.MethodHandleType, entry.MethodCallEntry.FunctionPointer), out fresh);
        }

        public DangerousMemoryViewProviderBuilder<Reference, nint>.Provider
        GetRootTableProvider()
        {
            if (_rootTableProvider.IsNull)
            {
                _rootTableProvider = GetOrAddMemoryKey(default, out _).Value;
            }

            return _rootTableProvider;
        }

        public LowLevelKeyValuePair<int, DangerousMemoryViewProviderBuilder<Reference, nint>.Provider>
        GetOrAddNodeKey(TNodeKey nodeKey, out DangerousMemoryViewProviderBuilderTheory.AddAndBuildFresh fresh) =>
        GetOrAddMemoryKey(new(nodeKey), out fresh);

        public AdjacentTableView<TEdgeLabel, TNodeKey, TReceiver> DangerousGetAdjacentTableView(TNodeKey nodeKey)
        {
            var provider = GetOrAddNodeKey(nodeKey, out _).Value;
            return provider.DangerousGetMemoryView<LowLevelKeyValuePair<TEdgeLabel, TNodeKey>, AdjacentTableView<TEdgeLabel, TNodeKey, TReceiver>>();
        }

        public LowLevelKeyValuePair<TEdgeLabel, TNodeKey> AddOrUpdateEdge(AdjacentTableView<TEdgeLabel, TNodeKey, TReceiver> table, TEdgeLabel edgeLabel, TNodeKey headNode)
        {
            for (int i = 0; i < table.Length; i++)
            {
                ref var entry = ref table[i];
                if (entry.Key.Equals(edgeLabel))
                {
                    entry.Value = headNode;
                    return entry;
                }
            }
            LowLevelKeyValuePair<TEdgeLabel, TNodeKey> newEntry = new(edgeLabel, headNode);
            table.Add(in newEntry);
            return newEntry;
        }
    }
}
