
using System.Runtime.InteropServices;

namespace DscTool.Infrastructure;

[StructLayout(LayoutKind.Sequential)]
public readonly struct PackedDigraph<TNodeKey, TEdgeLabel, TNodeValue>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    public readonly PackedMap<TNodeKey, NodeValueAdjacentsPair<TNodeKey, TEdgeLabel, TNodeValue>> _map;

    public PackedDigraph(PackedMap<TNodeKey, NodeValueAdjacentsPair<TNodeKey, TEdgeLabel, TNodeValue>> map)
    {
        _map = map;
    }

    public bool CheckNodeKeysAreDistinct()
    {
        // O(n^2) search
        var span = _map.AsSpan;
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

    public ReadOnlyMemory<LabeledNode<TNodeKey, TEdgeLabel>> GetAdjacents(TNodeKey node) =>
        _map.GetEntryOrFallback(node).Value.Adjacents;

    private struct SearchCycleState
    {
        public bool Visited;
        public bool Finished;
    }

    public bool TrySearchCycle([NotNullWhen(true)] out ImmutableList<TNodeKey>? cycleNode)
    {
        // Reference: https://en.wikipedia.org/wiki/Cycle_(graph_theory)#Algorithm

        PackedMap<TNodeKey, SearchCycleState> statesMap = _map.SelectValue(static _ => new SearchCycleState());
        cycleNode = null;
        foreach (var entry in _map.AsSpan)
        {
            DepthFirstSearch(in this, in statesMap, entry.Key, ref cycleNode);
            if (cycleNode is not null) {return true;}
        }
        
        return false;

        static void DepthFirstSearch
        (
            scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TNodeValue> self,
            scoped ref readonly PackedMap<TNodeKey, SearchCycleState> stateMap, 
            TNodeKey node, 
            [NotNullWhen(true)] ref ImmutableList<TNodeKey>? cycleNode
        )
        {
            ref SearchCycleState state = ref stateMap.GetValueRef(node);
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
                DepthFirstSearch(in self, in stateMap, adj.Node, ref cycleNode);
            }
            state.Finished = true;
        }
    }
}
