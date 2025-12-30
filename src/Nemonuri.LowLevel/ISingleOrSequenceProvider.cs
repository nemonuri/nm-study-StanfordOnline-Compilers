
namespace Nemonuri.LowLevel;

public interface ISingleOrMemoryViewProvider<T, TMemoryView>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
    where TMemoryView : IMemoryView<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
{
    bool GetSingleOrMemory
    (
        [NotNullWhen(true)] scoped ref T? single, 
        [NotNullWhen(false)] scoped ref TMemoryView? sequence
    );
}

public unsafe readonly struct SingleOrMemoryViewProviderHandle<TReceiver, T, TSequence>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
    where TSequence : IMemoryView<T>
#if NET9_0_OR_GREATER
    , allows ref struct
    where TReceiver : allows ref struct
#endif
{
    private readonly delegate*<ref TReceiver, out T?, out TSequence?, bool> _pGetSingleOrSequence;

    public SingleOrMemoryViewProviderHandle(delegate*<ref TReceiver, out T?, out TSequence?, bool> pGetSingleOrSequence)
    {
        _pGetSingleOrSequence = pGetSingleOrSequence;
    }

    public bool GetSingleOrSequence
    (
        ref TReceiver receiver,
        [NotNullWhen(true)] scoped ref T? single, 
        [NotNullWhen(false)] scoped ref TSequence? sequence
    )
    {
#pragma warning disable CS9094
        return _pGetSingleOrSequence(ref receiver, out single, out sequence);
#pragma warning restore CS9094
    }
}

public struct SingleOrSequenceProviderReceiver<TReceiver, T, TSequence> : ISingleOrMemoryViewProvider<T, TSequence>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
    where TSequence : IMemoryView<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
{
    private TReceiver _receiver;
    private readonly SingleOrMemoryViewProviderHandle<TReceiver, T, TSequence> _handle;

    public SingleOrSequenceProviderReceiver(TReceiver receiver, SingleOrMemoryViewProviderHandle<TReceiver, T, TSequence> handle)
    {
        _receiver = receiver;
        _handle = handle;
    }

    public bool GetSingleOrMemory([NotNullWhen(true)] scoped ref T? single, [NotNullWhen(false)] scoped ref TSequence? sequence)
    {
        return _handle.GetSingleOrSequence(ref _receiver, ref single, ref sequence);
    }
}

public struct SingleOrMemoryViewProviderReceiver<TReceiver, T> : ISingleOrMemoryViewProvider<T, MemoryViewReceiver<TReceiver, T>>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    private TReceiver _receiver;
    private readonly SingleOrMemoryViewProviderHandle<TReceiver, T,  MemoryViewReceiver<TReceiver, T>> _handle;

    public SingleOrMemoryViewProviderReceiver(TReceiver receiver, SingleOrMemoryViewProviderHandle<TReceiver, T,  MemoryViewReceiver<TReceiver, T>> handle)
    {
        _receiver = receiver;
        _handle = handle;
    }

    public bool GetSingleOrMemory([NotNullWhen(true)] scoped ref T? single, [NotNullWhen(false)] scoped ref MemoryViewReceiver<TReceiver, T> sequence)
    {
        return _handle.GetSingleOrSequence(ref _receiver, ref single, ref sequence);
    }
}

#if NET9_0_OR_GREATER
public ref struct SingleOrSpanViewProviderReceiver<TReceiver, T> : ISingleOrMemoryViewProvider<T, SpanViewReceiver<TReceiver, T>>
    where T : allows ref struct
    where TReceiver : allows ref struct
{
    private TReceiver _receiver;
    private readonly SingleOrMemoryViewProviderHandle<TReceiver, T,  SpanViewReceiver<TReceiver, T>> _handle;

    public SingleOrSpanViewProviderReceiver(TReceiver receiver, SingleOrMemoryViewProviderHandle<TReceiver, T,  SpanViewReceiver<TReceiver, T>> handle)
    {
        _receiver = receiver;
        _handle = handle;
    }

    public bool GetSingleOrMemory([NotNullWhen(true)] scoped ref T? single, [NotNullWhen(false)] scoped ref SpanViewReceiver<TReceiver, T> sequence)
    {
        return _handle.GetSingleOrSequence(ref _receiver, ref single, ref sequence);
    }
}
#endif