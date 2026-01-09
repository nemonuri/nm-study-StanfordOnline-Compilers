
namespace Nemonuri.LowLevel.Primitives.DotNet;

internal static class TypeInfoAddressCache<T>
{
    private const int None = -1;

    private static readonly int s_addressOrNone;

    static TypeInfoAddressCache()
    {
        RuntimeTypeHandle rth = typeof(T).TypeHandle;
        s_addressOrNone = RuntimeTypeTheory.CanGetOrAdd(rth, throwIfNot: false) ? RuntimeTypeTheory.GetOrAddAddress(rth) : None;
    }

    public static bool TryGetAddress(out int address) => (address = s_addressOrNone) is not None;
}
