
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
    private readonly delegate*<ref TReceiver, out TMemoryView, void> _getMemoryViewImpl; // unique field

    public MemoryViewProviderHandle(delegate*<ref TReceiver, out TMemoryView, void> getMemoryViewImpl)
    {
        LowLevelGuard.IsNotNull(getMemoryViewImpl);
        _getMemoryViewImpl = getMemoryViewImpl;
    }

    public void GetMemoryView(ref TReceiver receiver, out TMemoryView memoryView) => 
        _getMemoryViewImpl(ref receiver, out memoryView);
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct DangerousMemoryViewProviderHandle
{
    private readonly nint _handle;

    public static DangerousMemoryViewProviderHandle Wrap<TReceiver, T, TMemoryView>(in MemoryViewProviderHandle<TReceiver, T, TMemoryView> handle) 
    where TMemoryView : IMemoryView<T>
#if NET9_0_OR_GREATER
    ,allows ref struct
    where T : allows ref struct
    where TReceiver : allows ref struct
#endif
        =>
        UnsafeReadOnly.As<MemoryViewProviderHandle<TReceiver, T, TMemoryView>, DangerousMemoryViewProviderHandle>(in handle);
    
    public MemoryViewProviderHandle<TReceiver, T, TMemoryView> DangerousUnwrap<TReceiver, T, TMemoryView>()
    where TMemoryView : IMemoryView<T>
#if NET9_0_OR_GREATER
    ,allows ref struct
    where T : allows ref struct
    where TReceiver : allows ref struct
#endif
        =>
        UnsafeReadOnly.As<DangerousMemoryViewProviderHandle, MemoryViewProviderHandle<TReceiver, T, TMemoryView>>(in this);
}

[StructLayout(LayoutKind.Sequential)]
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

    [UnscopedRef] ref readonly TReceiver Receiver => ref _receiver;
    public readonly MemoryViewProviderHandle<TReceiver, T, TMemoryView> Handle => _handle;
}

public struct MemoryViewProviderReceiver<TReceiver, T> :
    IMemoryViewProvider<T, MemoryViewReceiver<TReceiver, T>>
{
    private TReceiver _receiver;
    private readonly MemoryViewProviderHandle<TReceiver, T, MemoryViewReceiver<TReceiver, T>> _handle;

    public MemoryViewProviderReceiver(TReceiver receiver, MemoryViewProviderHandle<TReceiver, T, MemoryViewReceiver<TReceiver, T>> handle)
    {
        _receiver = receiver; _handle = handle;
    }

    public void GetMemoryView(out MemoryViewReceiver<TReceiver, T> memoryView) => _handle.GetMemoryView(ref _receiver, out memoryView);
}

public unsafe struct DangerousMemoryViewProviderReceiver<TReceiver>
{
    private TReceiver _receiver;
    private readonly RuntimeTypeHandle _handleType;
    private readonly delegate*<in TReceiver, DangerousMemoryViewProviderHandle> _handleSelector;

    private DangerousMemoryViewProviderReceiver(in TReceiver receiver, RuntimeTypeHandle handleType, delegate*<in TReceiver, DangerousMemoryViewProviderHandle> handleSelector)
    {
        _receiver = receiver; 
        _handleType = handleType;
        _handleSelector = handleSelector;
    }

    public static DangerousMemoryViewProviderReceiver<TReceiver> Create<T, TMemoryView>(in TReceiver receiver, delegate*<in TReceiver, DangerousMemoryViewProviderHandle> handleSelector)
        where TMemoryView : IMemoryView<T>
#if NET9_0_OR_GREATER
        ,allows ref struct
        where T : allows ref struct
#endif
    {
        LowLevelGuard.IsNotNull(handleSelector);

        return new(in receiver, typeof(MemoryViewProviderHandle<TReceiver, T, TMemoryView>).TypeHandle, handleSelector);
    }

    [UnscopedRef]
    public void DangerousGetMemoryView<T, TMemoryView>(out TMemoryView memoryView)
        where TMemoryView : IMemoryView<T>
#if NET9_0_OR_GREATER
        ,allows ref struct
        where T : allows ref struct
#endif
    {
        if (!_handleType.Equals(typeof(MemoryViewProviderHandle<TReceiver, T, TMemoryView>).TypeHandle))
        {
            throw new InvalidCastException($"Desired = ${typeof(MemoryViewProviderHandle<TReceiver, T, TMemoryView>)}, Actual = ${Type.GetTypeFromHandle(_handleType)}");
        }

        DangerousMemoryViewProviderHandle handle = _handleSelector(in _receiver);
        UnsafeReadOnly.As<DangerousMemoryViewProviderHandle, MemoryViewProviderHandle<TReceiver, T, TMemoryView>>(in handle).GetMemoryView(ref _receiver, out memoryView);
    }
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
