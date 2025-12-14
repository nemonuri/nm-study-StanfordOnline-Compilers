using System.Runtime.CompilerServices;

namespace DscTool;

public static class UnsafeReadOnly
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly TTo As<TFrom, TTo>(ref readonly TFrom from) =>
        ref Unsafe.As<TFrom, TTo>(ref Unsafe.AsRef(in from));
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullRef<T>(ref readonly T source) =>
        Unsafe.IsNullRef(ref Unsafe.AsRef(in source));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotNullRef<T>(ref readonly T source) => !IsNullRef(in source);
}
