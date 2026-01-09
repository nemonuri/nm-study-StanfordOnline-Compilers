
namespace Nemonuri.LowLevel.Primitives.DotNet;

internal static class RuntimeFieldTheory
{
    private static GrowableArray<FieldInfo> s_fieldInfos = new(4);

    internal static Span<FieldInfo> FieldInfos => s_fieldInfos.AsSpan;

    private volatile static ConcurrentDictionary<RuntimeFieldAndTypeHandle, int>? s_fieldInfoStore;

    private static ConcurrentDictionary<RuntimeFieldAndTypeHandle, int> FieldInfoStore =>
      s_fieldInfoStore ??= Interlocked.CompareExchange(ref s_fieldInfoStore, new(), null) ?? s_fieldInfoStore;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetOrAddAddress(RuntimeFieldAndTypeHandle handle, int previousFieldAddressOrNone)
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

    internal static int AddFields(RuntimeTypeHandle typeHandle, ReadOnlySpan<RuntimeFieldHandle> fieldHandles)
    {
        int fieldHandlesLength = fieldHandles.Length;
        int fieldInfosLength = s_fieldInfos.AddDefaultsAndGetLength(fieldHandlesLength);
        int startAddress = fieldInfosLength - fieldHandlesLength;

        for (int i = 0; i < fieldHandlesLength; i++)
        {
            RuntimeFieldAndTypeHandle rfth = new(fieldHandles[i], typeHandle);
            FieldInfoStore.TryAdd(rfth, startAddress+i);
        }

        return startAddress;
    }

}
