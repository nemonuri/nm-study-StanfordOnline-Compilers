
namespace Nemonuri.LowLevel;

public interface ITree<TLeaf, TNode, TNodeSequence, TChildrenProvider> 
    where TNode : IRefBox<LowLevelChoice<TLeaf, TChildrenProvider>>
    where TNodeSequence : IMemoryView<TNode>
    where TChildrenProvider : ISingleOrSequenceProvider<TNode, TNodeSequence>
{
    [UnscopedRef] ref TNode RootNode {get;}
}

public unsafe readonly struct TreeHandle<TReceiver, TLeaf, TNode, TNodeSequence, TChildrenProvider> 
    where TNode : IRefBox<LowLevelChoice<TLeaf, TChildrenProvider>>
    where TNodeSequence : IMemoryView<TNode>
    where TChildrenProvider : ISingleOrSequenceProvider<TNode, TNodeSequence>
{
    private readonly delegate*<ref TReceiver, ref TNode> _pRootNode;

    public TreeHandle(delegate*<ref TReceiver, ref TNode> pRootNode)
    {
        LowLevelGuard.IsNotNull(pRootNode);
        _pRootNode = pRootNode;
    }

    public ref TNode GetRootNode(ref TReceiver receiver) => ref _pRootNode(ref receiver);
}

public struct TreeReceiver<TReceiver, TLeaf, TNode, TNodeSequence, TChildrenProvider> :
    ITree<TLeaf, TNode, TNodeSequence, TChildrenProvider>
    where TNode : IRefBox<LowLevelChoice<TLeaf, TChildrenProvider>>
    where TNodeSequence : IMemoryView<TNode>
    where TChildrenProvider : ISingleOrSequenceProvider<TNode, TNodeSequence>
{
    private TReceiver _receiver;
    private readonly TreeHandle<TReceiver, TLeaf, TNode, TNodeSequence, TChildrenProvider> _handle;

    public TreeReceiver(TReceiver receiver, TreeHandle<TReceiver, TLeaf, TNode, TNodeSequence, TChildrenProvider> handle)
    {
        _receiver = receiver;
        _handle = handle;
    }

    [UnscopedRef] public ref TNode RootNode => ref _handle.GetRootNode(ref _receiver);
}

//public readonly struct TreeNodeHandle

public readonly struct TreeProviderHandle<TReceiver, TLeaf, TNode, TNodeSequence> 
    where TNode : IRefBox<LowLevelChoice<TLeaf, SingleOrSequenceProviderReceiver<TReceiver, TNode, TNodeSequence>>>
    where TNodeSequence : IMemoryView<TNode>
{
    public readonly TreeHandle<TReceiver, TLeaf, TNode, TNodeSequence, SingleOrSequenceProviderReceiver<TReceiver, TNode, TNodeSequence>> RootProviderHandle;
    public readonly SingleOrSequenceProviderHandle<TReceiver, TNode, TNodeSequence> ChildrenProviderHandle;

    public TreeProviderHandle
    (
        TreeHandle<TReceiver, TLeaf, TNode, TNodeSequence, SingleOrSequenceProviderReceiver<TReceiver, TNode, TNodeSequence>> rootProviderHandle,
        SingleOrSequenceProviderHandle<TReceiver, TNode, TNodeSequence> childrenProviderHandle
    )
    {
        RootProviderHandle = rootProviderHandle;
        ChildrenProviderHandle = childrenProviderHandle;
    }
}
