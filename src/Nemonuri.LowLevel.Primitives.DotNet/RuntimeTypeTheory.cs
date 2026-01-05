using System.Collections.Concurrent;

namespace Nemonuri.LowLevel.Primitives.DotNet;

public static class RuntimeTypeTheory
{
    private static ConcurrentDictionary<RuntimeTypeHandle, TypeInfo>? _store;

    private static ConcurrentDictionary<RuntimeTypeHandle, TypeInfo> Store =>
        _store ??= Interlocked.CompareExchange(ref _store, new(new RuntimeTypeHandleEqualityComparer()), null) ?? _store;

    public static TypeInfo GetTypeInfo(RuntimeTypeHandle typeHandle)
    {
        return Store.GetOrAdd(key: typeHandle, valueFactory: static rth => new(rth));
    }

    public static bool IsUnmanaged<T>() => TypeInfo<T>.Instance.IsUnmanaged;

    public static bool IsUnmanaged(RuntimeTypeHandle typeHandle) => GetTypeInfo(typeHandle).IsUnmanaged;
}

internal class RuntimeTypeHandleEqualityComparer : IEqualityComparer<RuntimeTypeHandle>
{
    public bool Equals(RuntimeTypeHandle x, RuntimeTypeHandle y) => x.Equals(y);

    public int GetHashCode(RuntimeTypeHandle obj) => obj.GetHashCode();
}
