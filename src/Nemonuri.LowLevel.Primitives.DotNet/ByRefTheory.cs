namespace Nemonuri.LowLevel.Primitives.DotNet;

public static class ByRefTheory
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreSame<T>(scoped ref readonly T left, scoped ref readonly T right)
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
    {
        return
#if NET8_0_OR_GREATER
            Unsafe.AreSame(in left, in right);
#else
            Unsafe.AreSame(ref Unsafe.AsRef(in left), ref Unsafe.AsRef(in right));
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullRef<T>(scoped ref readonly T source)
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
    {
        return
#if NET8_0_OR_GREATER
            Unsafe.IsNullRef(in source);
#else
            Unsafe.IsNullRef(ref Unsafe.AsRef(in source));
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref byte UndefinedRef() => ref UndefinedLocation.Undefined;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T UndefinedRef<T>() 
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
        => ref Unsafe.As<byte, T>(ref UndefinedRef());


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUndefinedRef(scoped ref readonly byte location) => AreSame(in UndefinedRef(), in location);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUndefinedRef<T>(scoped ref readonly T location) 
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
        => IsUndefinedRef(ref Unsafe.As<T, byte>(ref Unsafe.AsRef<T>(in location)));
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUndefinedOrNullRef(ref readonly byte location) => IsUndefinedRef(in location) || IsNullRef(in location);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUndefinedOrNullRef<T>(ref readonly T location) 
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
        => IsUndefinedRef(in location) || IsNullRef(in location);
}
