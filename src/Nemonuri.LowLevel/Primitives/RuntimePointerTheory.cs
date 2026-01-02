
using Nemonuri.LowLevel.Runtime;

namespace Nemonuri.LowLevel.Primitives;

public static class RuntimePointerTheory
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref byte UndefinedRef() => ref UndefinedLocation.Undefined;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T UndefinedRef<T>() 
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
        => ref Unsafe.As<byte, T>(ref UndefinedRef());


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUndefinedRef(ref readonly byte location) => Unsafe.AreSame(in UndefinedRef(), in location);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUndefinedRef<T>(ref readonly T location) 
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
        => IsUndefinedRef(ref Unsafe.As<T, byte>(ref Unsafe.AsRef<T>(in location)));

    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUndefinedOrNullRef(ref readonly byte location) => IsUndefinedRef(in location) || Unsafe.IsNullRef(in location);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUndefinedOrNullRef<T>(ref readonly T location) 
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
        => IsUndefinedRef(in location) || Unsafe.IsNullRef(in location);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static void* UndefinedPointer() => Unsafe.AsPointer(ref UndefinedRef());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static bool IsUndefinedPointer(void* pointer) => pointer == UndefinedPointer();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static bool IsUndefinedOrNullPointer(void* pointer) => IsUndefinedPointer(pointer) || pointer == null;
}
