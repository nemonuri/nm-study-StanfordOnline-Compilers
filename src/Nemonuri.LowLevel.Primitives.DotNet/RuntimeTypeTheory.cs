using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Nemonuri.LowLevel.Primitives.DotNet;

public static class RuntimeTypeTheory
{
    private static ConcurrentDictionary<RuntimeTypeHandle, TypeInfo>? _store;

    private static ConcurrentDictionary<RuntimeTypeHandle, TypeInfo> Store =>
        _store ??= Interlocked.CompareExchange(ref _store, new(new RuntimeTypeHandleEqualityComparer()), null) ?? _store;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeInfo GetTypeInfo(RuntimeTypeHandle typeHandle)
    {
        return Store.GetOrAdd(key: typeHandle, valueFactory: CreateTypeInfoFromTypeHandle);
    }

    public static bool CanGetTypeInfo(RuntimeTypeHandle typeHandle, bool throwIfNot = false)
    {
        // Reference: https://github.com/dotnet/runtime/blob/main/src/mono/System.Private.CoreLib/src/System/Runtime/CompilerServices/RuntimeHelpers.Mono.cs

        if (typeHandle.Equals((RuntimeTypeHandle)default))
        {
            return throwIfNot ?
                throw new ArgumentNullException($"{nameof(typeHandle)} is null. Value = {typeHandle.Value}") : false;
        }

        Type t = Type.GetTypeFromHandle(typeHandle);
        if (t.ContainsGenericParameters || t.IsGenericParameter || t == typeof(void))
        {
            return throwIfNot ?
                throw new ArgumentException($"Not supported type. Type = {t}") : false;
        }

        return true;
    }

    private static TypeInfo CreateTypeInfoFromTypeHandle(RuntimeTypeHandle typeHandle)
    {
        CanGetTypeInfo(typeHandle, throwIfNot: true);
        return new (typeHandle);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUnmanaged<T>() => 
#if NETSTANDARD2_1_OR_GREATER
        !RuntimeHelpers.IsReferenceOrContainsReferences<T>();
#else
        TypeInfo<T>.Instance.IsUnmanaged;
#endif
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUnmanaged(RuntimeTypeHandle typeHandle) => GetTypeInfo(typeHandle).IsUnmanaged;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOf(RuntimeTypeHandle typeHandle)
    {  
        return
#if NET9_0_OR_GREATER
            RuntimeHelpers.SizeOf(typeHandle);
#else
            GetTypeInfo(typeHandle).Size;
#endif
    }
}

internal class RuntimeTypeHandleEqualityComparer : IEqualityComparer<RuntimeTypeHandle>
{
    public bool Equals(RuntimeTypeHandle x, RuntimeTypeHandle y) => x.Equals(y);

    public int GetHashCode(RuntimeTypeHandle obj) => obj.GetHashCode();
}
