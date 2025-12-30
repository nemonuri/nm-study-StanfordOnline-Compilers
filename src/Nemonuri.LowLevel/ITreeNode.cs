
namespace Nemonuri.LowLevel;

public interface ITreeNode<TLeaf, TNode, TNodeSequence, TChildrenProvider> :
    IRefBox<LowLevelChoice<TLeaf, TChildrenProvider>>
    where TNode : ITreeNode<TLeaf, TNode, TNodeSequence, TChildrenProvider>
    where TNodeSequence : IMemoryView<TNode>
    where TChildrenProvider : ISingleOrSequenceProvider<TNode, TNodeSequence>
{}

public struct TreeNodeReceiver<TReceiver, TLeaf, TNodeSequence, TChildrenProvider> :
    ITreeNode<TLeaf, TreeNodeReceiver<TReceiver, TLeaf, TNodeSequence, TChildrenProvider>, TNodeSequence, TChildrenProvider>
    where TNodeSequence : IMemoryView<TreeNodeReceiver<TReceiver, TLeaf, TNodeSequence, TChildrenProvider>>
    where TChildrenProvider : ISingleOrSequenceProvider<TreeNodeReceiver<TReceiver, TLeaf, TNodeSequence, TChildrenProvider>, TNodeSequence>
{
    private RefBoxReceiver<TReceiver, LowLevelChoice<TLeaf, TChildrenProvider>> _refBoxReceiver;

    public TreeNodeReceiver(RefBoxReceiver<TReceiver, LowLevelChoice<TLeaf, TChildrenProvider>> refBoxReceiver)
    {
        _refBoxReceiver = refBoxReceiver;
    }

    public TreeNodeReceiver(TReceiver receiver, RefBoxHandle<TReceiver, LowLevelChoice<TLeaf, TChildrenProvider>> handle) :
        this(new(receiver, handle))
    {
    }

    [UnscopedRef] public ref LowLevelChoice<TLeaf, TChildrenProvider> Value => ref _refBoxReceiver.Value;
}
