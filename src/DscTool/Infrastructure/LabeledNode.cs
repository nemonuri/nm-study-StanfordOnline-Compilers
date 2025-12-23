
namespace DscTool.Infrastructure;

public readonly struct LabeledNode<TNode, TLabel>
    where TNode : IEquatable<TNode>
    where TLabel : IEquatable<TLabel>
{
    public readonly TLabel Label;
    public readonly TNode Node;

    public LabeledNode(TLabel label, TNode node)
    {
        Label = label;
        Node = node;
    }
}

