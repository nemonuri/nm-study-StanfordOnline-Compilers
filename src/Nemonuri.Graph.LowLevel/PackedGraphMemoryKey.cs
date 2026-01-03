
using Lt = Nemonuri.LowLevel.LowLevelChoiceTagger;

namespace Nemonuri.Graph.LowLevel;

public readonly record struct PackedGraphMemoryKey<TNodeKey, TEdgeLabel>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    private readonly TNodeKey _nodeKey;
    private readonly LowLevelChoice<TEdgeLabel> _edgeLabel;

    public PackedGraphMemoryKey(TNodeKey nodekey)
    {
        _nodeKey = nodekey;
        _edgeLabel = Lt.None;
    }

    public PackedGraphMemoryKey(TNodeKey nodekey, TEdgeLabel edgeLabel)
    {
        _nodeKey = nodekey;
        _edgeLabel = Lt.Choice1(edgeLabel);
    }

    public bool HasEdgeLabel => _edgeLabel.IsChoice1;

    [UnscopedRef]
    public ref readonly TNodeKey NodeKey => ref _nodeKey;

    [UnscopedRef]
    public ref readonly TEdgeLabel EdgeLabel
    {
        get
        {
            Guard.IsTrue(HasEdgeLabel);
            return ref _edgeLabel.Choice1.Value;
        }
    }
}