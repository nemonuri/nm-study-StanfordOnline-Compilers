using System.Diagnostics;

namespace Nemonuri.LowLevel.Primitives.DotNet;

[StructLayout(LayoutKind.Sequential)]
public readonly struct UnmanagedTypeInfo
{
    public readonly nint TypeHandleValue;
    public readonly int Size;

    private UnmanagedTypeInfo(Type type, int size)
    {
        Debug.Assert(type.IsPrimitive);

        TypeHandleValue = type.TypeHandle.Value;
        Size = size;
    }

    public static UnmanagedTypeInfo Create<T>() where T : unmanaged
    {
        return new(typeof(T), Unsafe.SizeOf<T>());
    }
}
