
using System.Diagnostics;
using Nemonuri.LowLevel.Primitives;

namespace Nemonuri.LowLevel;

[StructLayout(LayoutKind.Sequential)]
public struct TypedUnmanagedBox<T> where T : unmanaged
{
    public readonly RuntimeTypeHandle TypeHandle;
    private T _valueBox;

    internal TypedUnmanagedBox(RuntimeTypeHandle typeHandle, T valueBox)
    {
        TypeHandle = typeHandle;
        _valueBox = valueBox;
    }

    public static TypedUnmanagedBox<T> Box<T2>(in T2 boxing) where T2 : unmanaged
    {
        Debug.Assert( RuntimeTypeTheory.AreSizeEqual<T, T2>() );
        T valueBox = UnsafeReadOnly.As<T2, T>(in boxing);
        return new(typeof(T2).TypeHandle, valueBox);
    }

    [UnscopedRef]
    public ref T2 DangerousUnbox<T2>() where T2 : unmanaged
    {
        Debug.Assert(Unsafe.SizeOf<T>() == Unsafe.SizeOf<T2>());
        if (!TypeHandle.Equals(typeof(T2).TypeHandle)) { throw new InvalidCastException(); }
        return ref Unsafe.As<T, T2>(ref _valueBox);
    }
}
