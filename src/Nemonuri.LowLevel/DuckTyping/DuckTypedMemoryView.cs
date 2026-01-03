
namespace Nemonuri.LowLevel.DuckTyping;


public struct DuckTypedMemoryView<TReceiver, T> : 
    IMemoryView<T>, 
    IMaybeSupportsRawSpan<T>, 
    IDuckTypeReceiver<TReceiver>
{
    private TReceiver _receiver;
    private readonly MemoryViewHandle<TReceiver, T> _memoryViewhandle;
    private readonly MaybeSupportsRawSpanHandle<TReceiver, T> _rawSpanHandle;

    public DuckTypedMemoryView
    (
        TReceiver receiver, 
        MemoryViewHandle<TReceiver, T> memoryViewhandle,
        MaybeSupportsRawSpanHandle<TReceiver, T> rawSpanHandle = default
    )
    {
        _receiver = receiver;
        _memoryViewhandle = memoryViewhandle;
        _rawSpanHandle = rawSpanHandle;
    }

    public readonly int Length => _memoryViewhandle.GetLength(in _receiver);

    [UnscopedRef] public ref T this[int index] => ref _memoryViewhandle.GetItem(ref _receiver, index);

    public readonly bool SupportsRawSpan => _rawSpanHandle.SupportsRawSpan;

    [UnscopedRef] public Span<T> AsSpan => _rawSpanHandle.AsSpan(ref _receiver);

    [UnscopedRef] public ref readonly TReceiver Receiver => ref _receiver;
}
