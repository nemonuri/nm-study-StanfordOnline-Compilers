
namespace Nemonuri.LowLevel.Primitives.DotNet;

public static class RuntimeTypeTheory
{
    private static GrowableArray<TypeInfo> s_typeInfos = new(4);

    internal static ReadOnlySpan<TypeInfo> TypeInfos => s_typeInfos.AsSpan;

    private static volatile ConcurrentDictionary<RuntimeTypeHandle, int>? s_typeInfoStore;

    private static ConcurrentDictionary<RuntimeTypeHandle, int> TypeInfoStore =>
        s_typeInfoStore ??= Interlocked.CompareExchange(ref s_typeInfoStore, new(RuntimeTypeHandleEqualityComparer.Instance), null) ?? s_typeInfoStore;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetOrAddAddress(RuntimeTypeHandle typeHandle)
    {
        return TypeInfoStore.GetOrAdd(key: typeHandle, valueFactory: CreateTypeInfoFromTypeHandle);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref readonly TypeInfo GetOrAdd(RuntimeTypeHandle typeHandle)
    {
        int typeInfoAddress = GetOrAddAddress(typeHandle);
        ref readonly TypeInfo ti = ref TypeInfos[typeInfoAddress];
        Debug.Assert( ti.Address == typeInfoAddress );
        return ref ti;
    }

    internal static bool CanGetOrAdd(RuntimeTypeHandle typeHandle, bool throwIfNot = false)
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
        CanGetOrAdd(typeHandle, throwIfNot: true);

        int nextAddress = s_typeInfos.Length;
        s_typeInfos.Add(new (nextAddress, typeHandle));
        return nextAddress;
    }

    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPrimitive(RuntimeTypeHandle rth) => PrimitiveValueTypeTheory.IsPrimitiveValueType(rth);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUnmanaged(RuntimeTypeHandle rth) => IsPrimitive(rth) || GetOrAdd(rth).IsUnmanaged;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUnmanaged<T>()
    {
#if NETSTANDARD2_1_OR_GREATER
        return !RuntimeHelpers.IsReferenceOrContainsReferences<T>();
#else
        if (typeof(T).IsPrimitive) {return true;}
        if (!TypeInfoAddressCache<T>.TryGetAddress(out int address)) {return false;}
        return TypeInfos[address].IsUnmanaged;
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValueType(RuntimeTypeHandle rth, [NotNullWhen(true)] out Type? dotnetType) => 
        (dotnetType = Type.GetTypeFromHandle(rth)) is { } typ && typ.IsValueType;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLayoutStableValueType(RuntimeTypeHandle rth, out bool isValueType, [NotNullWhen(true)] out Type? dotnetType)
    {
        if (!IsValueType(rth, out dotnetType)) {isValueType = false; return false;}
        isValueType = true;

        if (VoidTypeTheory.IsVoid(rth)) {return false;}
        if (dotnetType.IsAutoLayout) {return false;}

        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetStableSizeOrZero(RuntimeTypeHandle rth)
    {  
        if (!IsLayoutStableValueType(rth, out bool isValueType, out var dotnetType))
        {
            if (dotnetType is not { } dt) {return 0;}
            if (!isValueType) {return PrimitiveValueTypeTheory.IntPtrSize;}

            // Maybe System.Void
            return 0;
        }

#if NET9_0_OR_GREATER
           return RuntimeHelpers.SizeOf(rth);
#else
           int sizeCandidate = PrimitiveValueTypeTheory.GetSizeOrZero(rth);
           if (sizeCandidate > 0) {return sizeCandidate;}
           return GetOrAdd(rth).Size;
#endif
    }
}
