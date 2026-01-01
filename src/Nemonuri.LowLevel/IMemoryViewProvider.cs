
namespace Nemonuri.LowLevel;

public interface IMemoryViewProvider<T, TMemoryView>
    where TMemoryView : IMemoryView<T>
#if NET9_0_OR_GREATER
    ,allows ref struct
    where T : allows ref struct
#endif
{
    [UnscopedRef]
    void GetMemoryView(out TMemoryView memoryView);
}

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct MemoryViewProviderHandle<TReceiver, T, TMemoryView>
    where TMemoryView : IMemoryView<T>
#if NET9_0_OR_GREATER
    ,allows ref struct
    where T : allows ref struct
    where TReceiver : allows ref struct
#endif
{
    private readonly delegate*<ref TReceiver, out TMemoryView, void> _getMemoryViewImpl;

    public MemoryViewProviderHandle(delegate*<ref TReceiver, out TMemoryView, void> getMemoryViewImpl)
    {
        LowLevelGuard.IsNotNull(getMemoryViewImpl);
        _getMemoryViewImpl = getMemoryViewImpl;
    }

    public void GetMemoryView(ref TReceiver receiver, out TMemoryView memoryView) => 
        _getMemoryViewImpl(ref receiver, out memoryView);
}

public struct MemoryViewProviderReceiver<TReceiver, T, TMemoryView> :
    IMemoryViewProvider<T, TMemoryView>
    where TMemoryView : IMemoryView<T>
#if NET9_0_OR_GREATER
    ,allows ref struct
    where T : allows ref struct
#endif
{
    private TReceiver _receiver;
    private readonly MemoryViewProviderHandle<TReceiver, T, TMemoryView> _handle;

    public MemoryViewProviderReceiver(TReceiver receiver, MemoryViewProviderHandle<TReceiver, T, TMemoryView> handle)
    {
        _receiver = receiver;
        _handle = handle;
    }

    [UnscopedRef]
    public void GetMemoryView(out TMemoryView memoryView) => _handle.GetMemoryView(ref _receiver, out memoryView);
}

public readonly struct MemoryViewProviderReceiver<TReceiver, T> :
    IMemoryViewProvider<T, MemoryViewReceiver<TReceiver, T>>
{
    private readonly MemoryViewProviderReceiver<TReceiver, T, MemoryViewReceiver<TReceiver, T>> _provider;

    public MemoryViewProviderReceiver(MemoryViewProviderReceiver<TReceiver, T, MemoryViewReceiver<TReceiver, T>> provider)
    {
        _provider = provider;
    }

    public MemoryViewProviderReceiver(TReceiver receiver, MemoryViewProviderHandle<TReceiver, T, MemoryViewReceiver<TReceiver, T>> handle) :
        this(new(receiver, handle))
    {
    }

    public void GetMemoryView(out MemoryViewReceiver<TReceiver, T> memoryView) => _provider.GetMemoryView(out memoryView);
}

#if NET9_0_OR_GREATER
public ref struct SpanViewProviderReceiver<TReceiver, T> :
    IMemoryViewProvider<T, SpanViewReceiver<TReceiver, T>>
    where TReceiver : allows ref struct
{
    private TReceiver _receiver;
    private readonly MemoryViewProviderHandle<TReceiver, T, SpanViewReceiver<TReceiver, T>> _handle;

    public SpanViewProviderReceiver(TReceiver receiver, MemoryViewProviderHandle<TReceiver, T, SpanViewReceiver<TReceiver, T>> handle)
    {
        _receiver = receiver;
        _handle = handle;
    }

    [UnscopedRef]
    public void GetMemoryView(out SpanViewReceiver<TReceiver, T> memoryView) => _handle.GetMemoryView(ref _receiver, out memoryView);
}
#endif
