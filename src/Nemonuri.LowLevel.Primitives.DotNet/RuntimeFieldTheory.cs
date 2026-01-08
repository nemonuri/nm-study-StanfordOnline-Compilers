
namespace Nemonuri.LowLevel.Primitives.DotNet;

internal static class RuntimeFieldTheory
{
    private static GrowableArray<FieldInfo> s_fieldInfos = new(4);

    public static ReadOnlySpan<FieldInfo> FieldInfos => s_fieldInfos.AsSpan;

    [field: AllowNull, MaybeNull]
    private static ConcurrentDictionary<RuntimeFieldAndTypeHandle, int> FieldInfoStore =>
      field ??= Interlocked.CompareExchange(ref field, new(), null) ?? field;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetOrAddFieldInfoAddress(RuntimeFieldAndTypeHandle handle, int previousFieldAddressOrNone)
    {
#if NETSTANDARD2_1_OR_GREATER
        return FieldInfoStore.GetOrAdd<int>(key: handle, valueFactory: CreateAndAddFieldInfo, factoryArgument: previousFieldAddressOrNone);
#else
        return Polyfills.Polyfill.GetOrAdd<RuntimeFieldAndTypeHandle, int, int>(FieldInfoStore, key: handle, valueFactory: CreateAndAddFieldInfo, factoryArgument: previousFieldAddressOrNone);
#endif
    }

    public static int CreateAndAddFieldInfo(RuntimeFieldAndTypeHandle handle, int previousFieldAddressOrNone)
    {
        int nextAddress = s_fieldInfos.Length;
        s_fieldInfos.Add(new(nextAddress, handle, previousFieldAddressOrNone));
        return nextAddress;
    }

}
