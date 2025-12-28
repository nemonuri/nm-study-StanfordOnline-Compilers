
namespace Nemonuri.LowLevel;

public readonly ref struct RefLike<T>
{
    private readonly object? _managedBase;
    private readonly Span<byte> _unmanagedBase;
    private readonly RefLikeSelectorHandle<nint, byte, T> _refSelector;
    private readonly nint _pConfig;

    internal RefLike(object managedBase, RefLikeSelectorHandle<nint, byte, T> refSelector, nint pConfig)
    {
        _managedBase = managedBase;
        _unmanagedBase = [];
        _refSelector = refSelector;
        _pConfig = pConfig;
    }

    internal RefLike(Span<byte> unmanagedBase, RefLikeSelectorHandle<nint, byte, T> refSelector, nint pConfig)
    {
        _managedBase = null;
        _unmanagedBase = unmanagedBase;
        _refSelector = refSelector;
        _pConfig = pConfig;
    }

    [UnscopedRef]
    public ref T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (_managedBase is not null)
            {
                return ref _refSelector.Invoke(in _pConfig, ref Unsafe.As<object, byte>(ref Unsafe.AsRef(in _managedBase)));
            }
            else
            {
                return ref _refSelector.Invoke(in _pConfig, ref MemoryMarshal.GetReference(_unmanagedBase));
            }
        }
    }
}


public static class RefLike
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct ManagedBaseAndSelector<T>(object managedBase, RefSelectorHandle<object, T> selector)
    {
        public object ManagedBase = managedBase;
        public readonly RefSelectorHandle<object, T> Selector = selector;
    }

    public unsafe static RefLike<T> FromManaged<T>(object managedBase, delegate*<object, ref T> selector)
    {
        static ref T SelectorImpl(in nint voidSelector, ref byte byteSource)
        {
            ref var source = ref Unsafe.As<byte, object>(ref byteSource);
            var selector = (delegate*<object, ref T>)voidSelector;
            return ref selector(source);
        }

        Guard.IsNotNull(managedBase);
        LowLevelGuard.IsNotNull(selector);
        return new RefLike<T>(managedBase, new(&SelectorImpl), (nint)selector);
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct UnmanagedBaseAndSelector<TFrom, TTo>(TFrom unmanagedBase, RefSelectorHandle<TFrom, TTo> selector)
        where TFrom : unmanaged
        where TTo : unmanaged
    {
        public TFrom UnmanagedBase = unmanagedBase;
        public readonly RefSelectorHandle<TFrom, TTo> Selector = selector;
    }

    public unsafe static RefLike<TTo> FromUnmanaged<TFrom, TTo>(Span<TFrom> unmanagedBaseSingleton, delegate*<ref TFrom, ref TTo> selector)
        where TFrom : unmanaged
        where TTo : unmanaged
    {
        static ref TTo SelectorImpl(in nint voidSelector, ref byte byteSource)
        {
            ref var source = ref Unsafe.As<byte, TFrom>(ref byteSource);
            var selector = (delegate*<ref TFrom, ref TTo>)voidSelector;
            return ref selector(ref source);
        }

        Guard.IsEqualTo(unmanagedBaseSingleton.Length, 1);
        LowLevelGuard.IsNotNull(selector);
        return new RefLike<TTo>(MemoryMarshal.AsBytes(unmanagedBaseSingleton), new(&SelectorImpl), (nint)selector);
    }
}