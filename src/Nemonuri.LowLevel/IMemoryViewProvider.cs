
namespace Nemonuri.LowLevel;

public interface IMemoryViewProvider<T, TMemoryView> : IProviderInvokable<TMemoryView>
    where TMemoryView : IMemoryView<T>
#if NET9_0_OR_GREATER
    ,allows ref struct
    where T : allows ref struct
#endif
{
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct MemoryViewProviderHandle<TReceiver, T, TMemoryView>
    where TMemoryView : IMemoryView<T>
#if NET9_0_OR_GREATER
    ,allows ref struct
    where T : allows ref struct
    where TReceiver : allows ref struct
#endif
{
    private readonly ProviderHandle<TReceiver, TMemoryView> _providerHandle;

    public MemoryViewProviderHandle(ProviderHandle<TReceiver, TMemoryView> providerHandle)
    {
        _providerHandle = providerHandle;
    }

    public ref readonly TMemoryView InvokeProvider(in TReceiver receiver) => ref _providerHandle.InvokeProvider(in receiver)!;
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
public readonly struct MemoryViewProviderReceiver<TReceiver, T, TMemoryView> :
    IMemoryViewProvider<T, TMemoryView>
    where TMemoryView : IMemoryView<T>
#if NET9_0_OR_GREATER
    ,allows ref struct
    where T : allows ref struct
#endif
{
    private readonly TReceiver _receiver;
    private readonly MemoryViewProviderHandle<TReceiver, T, TMemoryView> _handle;

    public MemoryViewProviderReceiver(TReceiver receiver, MemoryViewProviderHandle<TReceiver, T, TMemoryView> handle)
    {
        _receiver = receiver;
        _handle = handle;
    }

    [UnscopedRef]
    public ref readonly TMemoryView? InvokeProvider() => ref _handle.InvokeProvider(in _receiver)!;
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

    [UnscopedRef]
    public ref readonly MemoryViewReceiver<TReceiver, T> InvokeProvider() => ref _handle.InvokeProvider(in _receiver);
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
    public ref readonly TMemoryView DangerousGetMemoryView<T, TMemoryView>()
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
        return ref UnsafeReadOnly.As<DangerousMemoryViewProviderHandle, MemoryViewProviderHandle<TReceiver, T, TMemoryView>>(in handle).InvokeProvider(in _receiver);
    }
}

#if NET9_0_OR_GREATER && false
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
