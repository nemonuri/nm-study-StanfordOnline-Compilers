
namespace Nemonuri.LowLevel.Primitives;

public static class RuntimeTypeTheory
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreSizeEqual<T1, T2>() => Unsafe.SizeOf<T1>() == Unsafe.SizeOf<T2>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreEqual<T>(RuntimeTypeHandle typeHandle) => typeHandle.Equals(typeof(T).TypeHandle);

    public static bool IsUnmanaged<T>()
    {
        return
#if NETSTANDARD2_1_OR_GREATER
            RuntimeHelpers.IsReferenceOrContainsReferences<T>()
#else
            Nemonuri.LowLevel.Runtime.TypeInfo<T>.IsReferenceOrContainsReferences
#endif
        ;
    }
}
