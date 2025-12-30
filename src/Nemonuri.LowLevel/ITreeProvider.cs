
namespace Nemonuri.LowLevel;

public interface ITreeProvider<TLeaf, TNode, TNodeSequence, TChildrenProvider> 
    where TNode : IRefBox<LowLevelChoice<TLeaf, TChildrenProvider>>
    where TNodeSequence : IMemoryView<TNode>
    where TChildrenProvider : ISingleOrSequenceProvider<TNode, TNodeSequence>
{
    [UnscopedRef] ref TNode RootNode {get;}
}

public unsafe readonly struct TreeProviderHandle<TReceiver, TLeaf, TNode, TNodeSequence, TChildrenProvider> 
    where TNode : IRefBox<LowLevelChoice<TLeaf, TChildrenProvider>>
    where TNodeSequence : IMemoryView<TNode>
    where TChildrenProvider : ISingleOrSequenceProvider<TNode, TNodeSequence>
{
    private readonly delegate*<ref TReceiver, ref TNode> _pRootNode;

    public TreeProviderHandle(delegate*<ref TReceiver, ref TNode> pRootNode)
    {
        LowLevelGuard.IsNotNull(pRootNode);
        _pRootNode = pRootNode;
    }

    public ref TNode GetRootNode(ref TReceiver receiver) => ref _pRootNode(ref receiver);
}

public struct TreeProviderReceiver<TReceiver, TLeaf, TNode, TNodeSequence, TChildrenProvider> :
    ITreeProvider<TLeaf, TNode, TNodeSequence, TChildrenProvider>
    where TNode : IRefBox<LowLevelChoice<TLeaf, TChildrenProvider>>
    where TNodeSequence : IMemoryView<TNode>
    where TChildrenProvider : ISingleOrSequenceProvider<TNode, TNodeSequence>
{
    private TReceiver _receiver;
    private readonly TreeProviderHandle<TReceiver, TLeaf, TNode, TNodeSequence, TChildrenProvider> _handle;

    public TreeProviderReceiver(TReceiver receiver, TreeProviderHandle<TReceiver, TLeaf, TNode, TNodeSequence, TChildrenProvider> handle)
    {
        _receiver = receiver;
        _handle = handle;
    }

    [UnscopedRef] public ref TNode RootNode => ref _handle.GetRootNode(ref _receiver);
}
