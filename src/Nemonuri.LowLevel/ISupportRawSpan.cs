namespace Nemonuri.LowLevel;

public interface IMaybeSupportsRawSpan<T>
{
    bool SupportsRawSpan {get;}
    [UnscopedRef] Span<T> AsSpan {get;}
}

public unsafe readonly struct MaybeSupportsRawSpanHandle<TReceiver, T>
#if NET9_0_OR_GREATER
    where TReceiver : allows ref struct
#endif
{
    private readonly delegate*<ref TReceiver, Span<T>> _pAsSpan;

    public MaybeSupportsRawSpanHandle(delegate*<ref TReceiver, Span<T>> pAsSpan = default)
    {
        _pAsSpan = pAsSpan;
    }

    public bool SupportsRawSpan => _pAsSpan != null;

    public Span<T> AsSpan(ref TReceiver receiver) => _pAsSpan(ref receiver);
}
