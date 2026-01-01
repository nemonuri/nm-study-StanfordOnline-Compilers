
namespace Nemonuri.LowLevel;

public interface ITreeNode<TLeaf, TNode, TNodeSequence, TChildrenProvider> :
    IRefBox<LowLevelChoice<TLeaf, TChildrenProvider>>
    where TNode : ITreeNode<TLeaf, TNode, TNodeSequence, TChildrenProvider>
    where TNodeSequence : IMemoryView<TNode>
    where TChildrenProvider : ISingleOrMemoryViewProvider<TNode, TNodeSequence>
{}

public struct TreeNodeReceiver<TReceiver, TLeaf, TNodeSequence, TChildrenProvider> :
    ITreeNode<TLeaf, TreeNodeReceiver<TReceiver, TLeaf, TNodeSequence, TChildrenProvider>, TNodeSequence, TChildrenProvider>
    where TNodeSequence : IMemoryView<TreeNodeReceiver<TReceiver, TLeaf, TNodeSequence, TChildrenProvider>>
    where TChildrenProvider : ISingleOrMemoryViewProvider<TreeNodeReceiver<TReceiver, TLeaf, TNodeSequence, TChildrenProvider>, TNodeSequence>
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

    [UnscopedRef] public ref LowLevelChoice<TLeaf, TChildrenProvider> RefValue => ref _refBoxReceiver.RefValue;
}

public struct TreeNodeReceiver<TReceiver, TLeaf> :
    ITreeNode<
        TLeaf, 
        TreeNodeReceiver<TReceiver, TLeaf>, 
        MemoryViewReceiver<TReceiver, TreeNodeReceiver<TReceiver, TLeaf>>,
        SingleOrMemoryViewProviderReceiver<TReceiver, TreeNodeReceiver<TReceiver, TLeaf>>>
{
    private RefBoxReceiver<TReceiver, LowLevelChoice<TLeaf, SingleOrMemoryViewProviderReceiver<TReceiver, TreeNodeReceiver<TReceiver, TLeaf>>>> _refBoxReceiver;

    public TreeNodeReceiver(RefBoxReceiver<TReceiver, LowLevelChoice<TLeaf, SingleOrMemoryViewProviderReceiver<TReceiver, TreeNodeReceiver<TReceiver, TLeaf>>>> refBoxReceiver)
    {
        _refBoxReceiver = refBoxReceiver;
    }

    public TreeNodeReceiver(TReceiver receiver, RefBoxHandle<TReceiver, LowLevelChoice<TLeaf, SingleOrMemoryViewProviderReceiver<TReceiver, TreeNodeReceiver<TReceiver, TLeaf>>>> handle) :
        this(new(receiver, handle))
    {
    }

    [UnscopedRef] public ref LowLevelChoice<TLeaf, SingleOrMemoryViewProviderReceiver<TReceiver, TreeNodeReceiver<TReceiver, TLeaf>>> RefValue => ref _refBoxReceiver.RefValue;
}
