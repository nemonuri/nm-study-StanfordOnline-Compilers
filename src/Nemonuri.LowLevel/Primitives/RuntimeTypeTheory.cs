
namespace Nemonuri.LowLevel.Primitives;

public static class RuntimeTypeTheory
{
    //--- typeof ----
    public static RuntimeTypeHandle TypeOfRuntimeTypeHandle {get;} = typeof(RuntimeTypeHandle).TypeHandle;

    //--- Reference: https://en.cppreference.com/w/c/header/stddef.html ---

    // ptrdiff_t
    public static RuntimeTypeHandle TypeOfPointerDifference {get;} = typeof(nint).TypeHandle;

    // size_t
    public static RuntimeTypeHandle TypeOfArrayIndex {get;} = typeof(int).TypeHandle;

    //---|

    public static RuntimeTypeHandle TypeOfObject {get;} = typeof(object).TypeHandle;

    public static RuntimeTypeHandle TypeOfObjectArray {get;} = typeof(object[]).TypeHandle;

    public static RuntimeTypeHandle TypeOfByteArray {get;} = typeof(byte[]).TypeHandle;
    //---|


    public static RuntimeTypeHandle Null {get;} = default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreEqual(RuntimeTypeHandle left, RuntimeTypeHandle right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreEqual<T>(RuntimeTypeHandle typeHandle) => AreEqual(typeof(T).TypeHandle, typeHandle);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreEqual<T1, T2>() => AreEqual(typeof(T1).TypeHandle, typeof(T2).TypeHandle);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNull(RuntimeTypeHandle handle) => AreEqual(handle, Null);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUnmanaged<T>() => 
#if NETSTANDARD2_1_OR_GREATER
        !RuntimeHelpers.IsReferenceOrContainsReferences<T>();
#else
        DotNet.RuntimeTypeTheory.IsUnmanaged<T>();
#endif
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUnmanaged(RuntimeTypeHandle typeHandle) => DotNet.RuntimeTypeTheory.IsUnmanaged(typeHandle);


    public static int SizeOf<T>() => Unsafe.SizeOf<T>();


    //--- null ---
    

    public static object? NullObject {get;} = null;

    public static nint IsZeroPointer {get;} = default;
    //---|

    //--- Equal ---


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreReferenceEqualObjects(object? left, object? right) => ReferenceEquals(left, right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreEqualPointer(nint left, nint right) => left == right;
    //---|



    


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreSizeEqual<T1, T2>() => Unsafe.SizeOf<T1>() == Unsafe.SizeOf<T2>();






    
}
