
namespace Nemonuri.LowLevel.Primitives.DotNet;

public static class RuntimeTypeTheory
{
    private static GrowableArray<TypeInfo> s_typeInfos = new(4);

    public static ReadOnlySpan<TypeInfo> TypeInfos => s_typeInfos.AsSpan;


    private static volatile ConcurrentDictionary<RuntimeTypeHandle, int>? s_typeInfoStore;

    private static ConcurrentDictionary<RuntimeTypeHandle, int> TypeInfoStore =>
        s_typeInfoStore ??= Interlocked.CompareExchange(ref s_typeInfoStore, new(RuntimeTypeHandleEqualityComparer.Instance), null) ?? s_typeInfoStore;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetTypeInfoAddress(RuntimeTypeHandle typeHandle)
    {
        return TypeInfoStore.GetOrAdd(key: typeHandle, valueFactory: CreateTypeInfoFromTypeHandle);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly TypeInfo GetTypeInfo(RuntimeTypeHandle typeHandle)
    {
        int typeInfoAddress = GetTypeInfoAddress(typeHandle);
        ref readonly TypeInfo ti = ref TypeInfos[typeInfoAddress];
        Debug.Assert( ti.Address == typeInfoAddress );
        return ref ti;
    }

    public static bool CanGetTypeInfo(RuntimeTypeHandle typeHandle, bool throwIfNot = false)
    {
        // Reference: https://github.com/dotnet/runtime/blob/main/src/mono/System.Private.CoreLib/src/System/Runtime/CompilerServices/RuntimeHelpers.Mono.cs

        if (typeHandle.Equals((RuntimeTypeHandle)default) || Type.GetTypeFromHandle(typeHandle) is not { } t)
        {
            return throwIfNot ?
                throw new ArgumentNullException($"{nameof(typeHandle)} is null. Value = {typeHandle.Value}") : false;
        }

        if (t.ContainsGenericParameters || t.IsGenericParameter || t == typeof(void) || t.IsPrimitive)
        {
            return throwIfNot ?
                throw new ArgumentException($"Not supported type. Type = {t}") : false;
        }

        return true;
    }

    private static int CreateTypeInfoFromTypeHandle(RuntimeTypeHandle typeHandle)
    {
        CanGetTypeInfo(typeHandle, throwIfNot: true);

        int nextAddress = s_typeInfos.Length;
        s_typeInfos.Add(new (nextAddress, typeHandle));
        return nextAddress;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUnmanaged<T>() => 
#if NET || NETSTANDARD2_1_OR_GREATER
        !RuntimeHelpers.IsReferenceOrContainsReferences<T>();
#else
        TypeInfo<T>.Instance.IsUnmanaged;
#endif
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUnmanaged(RuntimeTypeHandle typeHandle) => PrimitiveValueTypeTheory.IsPrimitiveValueType(typeHandle) || GetTypeInfo(typeHandle).IsUnmanaged;

    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSizeOrZero(RuntimeTypeHandle typeHandle)
    {  
        int sizeCandidate = PrimitiveValueTypeTheory.GetSizeOrZero(typeHandle);

        return
#if NET9_0_OR_GREATER
            RuntimeHelpers.SizeOf(typeHandle);
#else
            
            GetTypeInfo(typeHandle).Size;
#endif
    }
}
