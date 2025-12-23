
namespace DscTool.Infrastructure;

public readonly struct NodeValueAdjacentsPair<TNodeKey, TEdgeLabel, TNodeValue>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    public readonly TNodeValue NodeValue;
    public readonly ReadOnlyMemory<LabeledNode<TNodeKey, TEdgeLabel>> Adjacents;

    public NodeValueAdjacentsPair(TNodeValue nodeValue, ReadOnlyMemory<LabeledNode<TNodeKey, TEdgeLabel>> adjacents)
    {
        NodeValue = nodeValue;
        Adjacents = adjacents;
    }
}
