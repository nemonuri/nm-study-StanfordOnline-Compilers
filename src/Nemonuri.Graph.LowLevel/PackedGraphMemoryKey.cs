
using Lt = Nemonuri.LowLevel.LowLevelChoiceTagger;

namespace Nemonuri.Graph.LowLevel;

public readonly record struct PackedGraphMemoryKey<TNodeKey>
    where TNodeKey : IEquatable<TNodeKey>
{
    private readonly LowLevelChoice<TNodeKey> _nodeKey;

    public PackedGraphMemoryKey(TNodeKey nodekey)
    {
        _nodeKey = Lt.Choice1(nodekey);
    }

    public bool HasNodeKey => _nodeKey.IsChoice1;

    [UnscopedRef]
    public ref readonly TNodeKey NodeKey
    {
        get
        {
            Guard.IsTrue(HasNodeKey);
            return ref _nodeKey.Choice1.Value;
        }
    }
}