
namespace Nemonuri.LowLevel;

public interface ISingleOrSequenceProvider<T, TSequence>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
    where TSequence : IMemoryView<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
{
    bool GetSingleOrSequence
    (
        [NotNullWhen(true)] scoped ref T? single, 
        [NotNullWhen(false)] scoped ref TSequence? sequence
    );
}

public unsafe readonly struct SingleOrSequenceProviderHandle<TReceiver, T, TSequence>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
    where TSequence : IMemoryView<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
{
    private readonly delegate*<ref TReceiver, out T?, out TSequence?, bool> _pGetSingleOrSequence;

    public SingleOrSequenceProviderHandle(delegate*<ref TReceiver, out T?, out TSequence?, bool> pGetSingleOrSequence)
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

public struct SingleOrSequenceProviderReceiver<TReceiver, T, TSequence> : ISingleOrSequenceProvider<T, TSequence>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
    where TSequence : IMemoryView<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
{
    private TReceiver _receiver;
    private readonly SingleOrSequenceProviderHandle<TReceiver, T, TSequence> _handle;

    public SingleOrSequenceProviderReceiver(TReceiver receiver, SingleOrSequenceProviderHandle<TReceiver, T, TSequence> handle)
    {
        _receiver = receiver;
        _handle = handle;
    }

    public bool GetSingleOrSequence([NotNullWhen(true)] scoped ref T? single, [NotNullWhen(false)] scoped ref TSequence? sequence)
    {
        return _handle.GetSingleOrSequence(ref _receiver, ref single, ref sequence);
    }
}