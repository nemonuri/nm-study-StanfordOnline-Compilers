
using System.Runtime.InteropServices;

namespace DscTool.Infrastructure;

[StructLayout(LayoutKind.Sequential)]
public readonly struct PackedDigraph<TNodeKey, TEdgeLabel, TNodeValue>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    public readonly PackedMap<TNodeKey, NodeValueAdjacentsPair<TNodeKey, TEdgeLabel, TNodeValue>> PackedMap;

    public PackedDigraph(PackedMap<TNodeKey, NodeValueAdjacentsPair<TNodeKey, TEdgeLabel, TNodeValue>> map)
    {
        PackedMap = map;
    }

    public bool CheckNodeKeysAreDistinct()
    {
        // O(n^2) search
        var span = PackedMap.AsSpan;
        for (int i = 0; i < span.Length; i++)
        {
            TNodeKey cur = span[i].Key;
            for (int i2 = i+1; i2 < span.Length; i2++)
            {
                if (cur.Equals(span[i2].Key)) {return false;}
            }
        }
        return true;
    }

    public TNodeValue FallbackValue => PackedMap.Fallback.NodeValue;

    public Span<RawKeyValuePair<TNodeKey, NodeValueAdjacentsPair<TNodeKey, TEdgeLabel, TNodeValue>>> AsSpan => PackedMap.AsSpan;

    public RawKeyValuePair<OptionalKey<TNodeKey>, NodeValueAdjacentsPair<TNodeKey, TEdgeLabel, TNodeValue>> 
    GetEntryOrFallback(TNodeKey nodeKey) =>
        PackedMap.GetEntryOrFallback(nodeKey);

    public RawKeyValuePair<OptionalKey<TNodeKey>, NodeValueAdjacentsPair<TNodeKey, TEdgeLabel, TNodeValue>> 
    GetEntryOrFallback(OptionalKey<TNodeKey> nodeKey) =>
        PackedMap.GetEntryOrFallback(nodeKey);

    public ReadOnlyMemory<LabeledNode<TNodeKey, TEdgeLabel>> GetAdjacents(TNodeKey nodeKey) =>
        GetEntryOrFallback(nodeKey).Value.Adjacents;

    public PackedMap<TNodeKey, TNodeValue> GetNodeValuesAsPackedMap()
    {
        return PackedMap.SelectValue(static pair => pair.NodeValue);
    }

    public PackedMap<TEdgeLabel, OptionalKey<TNodeKey>> GetAdjacentsAsPackedMap(TNodeKey nodeKey)
    {
        var originalSpan = GetAdjacents(nodeKey).Span;
        var packedMapArray = new RawKeyValuePair<TEdgeLabel, OptionalKey<TNodeKey>>[originalSpan.Length];
        for (int i = 0; i < originalSpan.Length; i++)
        {
            ref readonly LabeledNode<TNodeKey, TEdgeLabel> labeledNode = ref originalSpan[i];
            packedMapArray[i] = new(key: labeledNode.Label, value: OptionalKeyTagger.Some(labeledNode.Node));
        }
        return new(memory: new(packedMapArray), fallback: OptionalKeyTagger.None);
    }

    public PackedMap<TEdgeLabel, OptionalKey<TNodeKey>> GetAdjacentsAsPackedMap(OptionalKey<TNodeKey> nodeKey)
    {
        if (!nodeKey.IsSome) { return new(memory: default, fallback: OptionalKeyTagger.None); }
        return GetAdjacentsAsPackedMap(nodeKey.GetSome());
    }

    private struct DfsState
    {
        public bool Visited;
        public bool Finished;

        public DfsState()
        {
            Visited=false; Finished=false;
        }
    }

    public bool TrySearchCycle([NotNullWhen(true)] out ImmutableList<TNodeKey>? cycleNode)
    {
        // Reference: https://en.wikipedia.org/wiki/Cycle_(graph_theory)#Algorithm

        PackedMap<TNodeKey, DfsState> statesMap = PackedMap.SelectValue(static _ => new DfsState());
        cycleNode = null;
        foreach (ref readonly var entry in AsSpan)
        {
            DepthFirstSearch(in this, in statesMap, entry.Key, ref cycleNode);
            if (cycleNode is not null) {return true;}
        }
        
        return false;

        static void DepthFirstSearch
        (
            scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TNodeValue> self,
            scoped ref readonly PackedMap<TNodeKey, DfsState> statesMap, 
            TNodeKey node, 
            [NotNullWhen(true)] ref ImmutableList<TNodeKey>? cycleNode
        )
        {
            ref DfsState state = ref statesMap.GetValueRef(node);
            if (state.Finished) {return;}
            if (state.Visited) 
            {
                cycleNode = cycleNode switch
                {
                    { } c => c.Add(node),
                    _ => [node]
                }; 
                return;
            }
            state.Visited = true;
            foreach (var adj in self.GetAdjacents(node).Span)
            {
                DepthFirstSearch(in self, in statesMap, adj.Node, ref cycleNode);
            }
            state.Finished = true;
        }
    }
}
