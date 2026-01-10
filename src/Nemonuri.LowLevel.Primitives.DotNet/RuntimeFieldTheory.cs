
namespace Nemonuri.LowLevel.Primitives.DotNet;

internal static class RuntimeFieldTheory
{
    private static GrowableArray<FieldInfo> s_fieldInfos = new(4);

    internal static Span<FieldInfo> FieldInfos => s_fieldInfos.AsSpan;

    private volatile static ConcurrentDictionary<RuntimeFieldAndTypeHandle, int>? s_fieldInfoStore;

    private static ConcurrentDictionary<RuntimeFieldAndTypeHandle, int> FieldInfoStore =>
      s_fieldInfoStore ??= Interlocked.CompareExchange(ref s_fieldInfoStore, new(), null) ?? s_fieldInfoStore;

    internal static int AddFields(RuntimeTypeHandle typeHandle, ReadOnlySpan<RuntimeFieldHandle> fieldHandles)
    {
        int fieldHandlesLength = fieldHandles.Length;
        int fieldInfosLength = s_fieldInfos.AddDefaultsAndGetLength(fieldHandlesLength);
        int startAddress = fieldInfosLength - fieldHandlesLength;

        for (int i = 0; i < fieldHandlesLength; i++)
        {
            RuntimeFieldAndTypeHandle key = new(fieldHandles[i], typeHandle);
            int desiredValue = startAddress+i;
            if (FieldInfoStore.GetOrAdd(key, desiredValue) != desiredValue)
            {
                FieldInfoStore[key] = desiredValue;
            }
        }

        return startAddress;
    }

}
