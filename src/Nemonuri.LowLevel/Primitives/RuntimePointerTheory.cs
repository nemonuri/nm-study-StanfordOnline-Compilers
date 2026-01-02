
using Nemonuri.LowLevel.Internal;

namespace Nemonuri.LowLevel.Primitives;

public static class RuntimePointerTheory
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref byte UndefinedRef() => ref UndefinedLocation.Undefined;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T UndefinedRef<T>() => ref Unsafe.As<byte, T>(ref UndefinedRef());


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUndefinedRef(ref readonly byte location) => Unsafe.AreSame(in UndefinedRef(), in location);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUndefinedRef<T>(ref readonly T location) => IsUndefinedRef(in UnsafeReadOnly.As<T, byte>(in location));

    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUndefinedOrNullRef(ref readonly byte location) => IsUndefinedRef(in location) && Unsafe.IsNullRef(in location);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUndefinedOrNullRef<T>(ref readonly T location) => IsUndefinedRef(in location) && Unsafe.IsNullRef(in location);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static void* UndefinedPointer() => Unsafe.AsPointer(ref UndefinedRef());
}