
namespace Nemonuri.LowLevel;

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct KeyedMemoryViewProviderHandle<TReceiver, TKey, T, TMemoryView>
    where TMemoryView : IMemoryView<T>
#if NET9_0_OR_GREATER
    ,allows ref struct
    where T : allows ref struct
    where TReceiver : allows ref struct
#endif
{
    private readonly delegate*<ref TReceiver, in TKey, out TMemoryView, void> _getMemoryViewImpl;

    public KeyedMemoryViewProviderHandle(delegate*<ref TReceiver, in TKey, out TMemoryView, void> getMemoryViewImpl)
    {
        LowLevelGuard.IsNotNull(getMemoryViewImpl);
        _getMemoryViewImpl = getMemoryViewImpl;
    }

    public void GetMemoryView(ref TReceiver receiver, in TKey key, out TMemoryView memoryView) => 
        _getMemoryViewImpl(ref receiver, in key, out memoryView);
}

public readonly struct KeyedMemoryViewProviderHandle<TReceiver, TKey>
{
    private readonly RuntimeTypeHandle _runtimeTypeHandle;
    private readonly nint _memoryViewProviderHandle;

    internal KeyedMemoryViewProviderHandle(RuntimeTypeHandle runtimeTypeHandle, nint memoryViewProviderHandle)
    {
        _runtimeTypeHandle = runtimeTypeHandle;
        _memoryViewProviderHandle = memoryViewProviderHandle;
    }

    public bool TryGetMemoryView<T, TMemoryView>(ref TReceiver receiver, in TKey key, [NotNullWhen(true)] out TMemoryView? memoryView)
    where TMemoryView : IMemoryView<T>
#if NET9_0_OR_GREATER
    ,allows ref struct
    where T : allows ref struct
#endif
    {
        if (!_runtimeTypeHandle.Equals(typeof(KeyedMemoryViewProviderHandle<TReceiver, TKey, T, TMemoryView>).TypeHandle)) 
            { memoryView = default; return false; }
        
        UnsafeReadOnly.As<nint, KeyedMemoryViewProviderHandle<TReceiver, TKey, T, TMemoryView>>(in _memoryViewProviderHandle).GetMemoryView(ref receiver, in key, out memoryView);
        return true;
    }
}

public static class KeyedMemoryViewProviderHandle
{
    extension<TReceiver, TKey, T, TMemoryView>(in KeyedMemoryViewProviderHandle<TReceiver, TKey, T, TMemoryView> memoryView)
        where TMemoryView : IMemoryView<T>
    #if NET9_0_OR_GREATER
        ,allows ref struct
        where T : allows ref struct
    #endif
    {
        public KeyedMemoryViewProviderHandle<TReceiver, TKey> Abstract()
        {
            RuntimeTypeHandle typeHandle = memoryView.GetType().TypeHandle;
            nint typeErased = UnsafeReadOnly.As<KeyedMemoryViewProviderHandle<TReceiver, TKey, T, TMemoryView>, nint>(in memoryView);
            return new(typeHandle, typeErased);
        }
    }

    extension<TReceiver, TKey>(in LowLevelKeyValuePair<TKey, KeyedMemoryViewProviderHandle<TReceiver, TKey>> pair)
    {
        public bool TryGetMemoryView<T, TMemoryView>(ref TReceiver receiver, [NotNullWhen(true)] out TMemoryView? memoryView)
            where TMemoryView : IMemoryView<T>
        #if NET9_0_OR_GREATER
            ,allows ref struct
            where T : allows ref struct
        #endif
        {
            return pair.Value.TryGetMemoryView<T, TMemoryView>(ref receiver, in pair.Key, out memoryView);
        }
    }
}


