using Nemonuri.LowLevel.Internal;

namespace Nemonuri.LowLevel;

public static class UnsafeReadOnly
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly TTo As<TFrom, TTo>(ref readonly TFrom from) 
#if NET9_0_OR_GREATER
    where TFrom : allows ref struct
    where TTo : allows ref struct
#endif
        =>
        ref Unsafe.As<TFrom, TTo>(ref Unsafe.AsRef(in from));
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullRef<T>(ref readonly T source) =>
        Unsafe.IsNullRef(ref Unsafe.AsRef(in source));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotNullRef<T>(ref readonly T source) => !IsNullRef(in source);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreNotNullRef<T>(ref readonly T left, ref readonly T right) => 
        IsNotNullRef(in left) && IsNotNullRef(in right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreSameRef<T>(ref readonly T left, ref readonly T right) =>
        Unsafe.AreSame(ref Unsafe.AsRef(in left), ref Unsafe.AsRef(in right));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreNotNullRefAndSame<T>(ref readonly T left, ref readonly T right) =>
        AreNotNullRef(in left, in right) && AreSameRef(in left, in right);
    
    public static ref readonly T? GetDefaultRef<T>() => ref Defaults<T>.Default;
}
