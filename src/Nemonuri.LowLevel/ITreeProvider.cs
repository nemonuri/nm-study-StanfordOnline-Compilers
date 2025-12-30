
namespace Nemonuri.LowLevel;

public interface ITreeProvider<T, TSequence> :
    ISingleOrSequenceProvider<T, TSequence>
    where T : ITreeProvider<T, TSequence>
#if NET9_0_OR_GREATER
    ,allows ref struct
#endif
    where TSequence : IMemoryView<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
{}

public readonly struct TreeProviderReceiver<TReceiver, T, TSequence> :
    ITreeProvider<T, TSequence>
    where T : ITreeProvider<T, TSequence>
#if NET9_0_OR_GREATER
    ,allows ref struct
#endif
    where TSequence : IMemoryView<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
{
    private readonly SingleOrSequenceProviderReceiver<TReceiver, T, TSequence> _receiver;

    public TreeProviderReceiver(SingleOrSequenceProviderReceiver<TReceiver, T, TSequence> receiver)
    {
        _receiver = receiver;
    }

    public TreeProviderReceiver(TReceiver receiver, SingleOrSequenceProviderHandle<TReceiver, T, TSequence> handle) :
        this(new(receiver, handle))
    {
    }

    public bool GetSingleOrSequence([NotNullWhen(true)] scoped ref T? single, [NotNullWhen(false)] scoped ref TSequence? sequence)
    {
        return _receiver.GetSingleOrSequence(ref single, ref sequence);
    }
}
