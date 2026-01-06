using System.Runtime.CompilerServices;

namespace Nemonuri.LowLevel.Primitives.DotNet.UnitTests;

public readonly struct TestTypeInfo
{
    public readonly Type TypeObject;
    public readonly int Size;

    private TestTypeInfo(Type type, int size)
    {
        TypeObject = type;
        Size = size;
    }

    public static TestTypeInfo Create<T>()
    {
        return new(typeof(T), Unsafe.SizeOf<T>());
    }
}
