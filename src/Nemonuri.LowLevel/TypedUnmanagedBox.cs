
using System.Diagnostics;
using Nemonuri.LowLevel.Primitives;

namespace Nemonuri.LowLevel;

[StructLayout(LayoutKind.Sequential)]
public struct TypedUnmanagedBox<T> where T : unmanaged
{
    public readonly RuntimeTypeHandle TypeHandle;
    private T _valueBox;

    public TypedUnmanagedBox(RuntimeTypeHandle typeHandle, T valueBox)
    {
        TypeHandle = typeHandle;
        _valueBox = valueBox;
    }

    public static TypedUnmanagedBox<T> Box<T2>(in T2 boxing) where T2 : unmanaged
    {
        ref T valueBox = ref UnmanagedValueTheory.BitCastOrUndefined<T2, T>(ref Unsafe.AsRef<T2>(in boxing));
        Debug.Assert( !RuntimePointerTheory.IsUndefinedOrNullRef(ref valueBox) );
        return new(typeof(T2).TypeHandle, valueBox);
    }

    [UnscopedRef]
    public ref T2 DangerousUnbox<T2>() where T2 : unmanaged
    {
        ref T2 result = ref UnmanagedValueTheory.BitCastOrUndefined<T, T2>(ref _valueBox);
        Debug.Assert( !RuntimePointerTheory.IsUndefinedOrNullRef(ref result) );
        return ref result;
    }

    [UnscopedRef]
    public ref readonly T BoxedValue => ref _valueBox;
}
