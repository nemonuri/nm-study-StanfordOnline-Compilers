
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
}
