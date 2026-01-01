
using CommunityToolkit.HighPerformance;
using RawValue =
        Nemonuri.LowLevel.SequentialLayoutValueTuple<
            Nemonuri.LowLevel.DangerousMemoryViewProviderReceiver<Nemonuri.LowLevel.MemoryViewConfig>, 
            Nemonuri.LowLevel.DangerousMemoryViewProviderHandle,
            Nemonuri.LowLevel.DangerousMemoryViewProviderReceiver<Nemonuri.LowLevel.LowLevelChoice<object, nint>>
        >;
using RawEntry = 
    Nemonuri.LowLevel.RawKeyValuePair<
        int, 
        Nemonuri.LowLevel.SequentialLayoutValueTuple<
            Nemonuri.LowLevel.DangerousMemoryViewProviderReceiver<Nemonuri.LowLevel.MemoryViewConfig>, 
            Nemonuri.LowLevel.DangerousMemoryViewProviderHandle,
            Nemonuri.LowLevel.DangerousMemoryViewProviderReceiver<Nemonuri.LowLevel.LowLevelChoice<object, nint>>
        >
    >;

namespace Nemonuri.LowLevel;

public readonly struct MemoryViewConfig : 
    IMemoryViewManager<int, MemoryViewConfig>,
    IConfig<Box<PackedTable<int, RawValue>.Builder>, int, MemoryViewConfig>
{
    public Box<PackedTable<int, RawValue>.Builder> SharedConfig {get;}
    private readonly int _key;

    internal MemoryViewConfig
    (
        Box<PackedTable<int, RawValue>.Builder> shardConfig,
        int key
    )
    {
        SharedConfig = shardConfig ?? PackedTable<int, RawValue>.CreateBuilder(0);
        _key = key;
    }

    public ref readonly int GetIndividualKeyRef(ref MemoryViewConfig receiver) => ref receiver.IndividualConfig;

    public int Length => SharedConfig.GetReference().Length;

    public ref RawKeyValuePair<int, DangerousMemoryViewProviderReceiver<MemoryViewConfig>> this[int index] => 
        ref Unsafe.As<RawEntry, RawKeyValuePair<int, DangerousMemoryViewProviderReceiver<MemoryViewConfig>>>(ref SharedConfig.GetReference()[index]);

    [UnscopedRef]
    public ref readonly int IndividualConfig => ref _key;

    public MemoryViewConfig WithNewIndividualConfig(in int individual) => new(SharedConfig, individual);

    private static DangerousMemoryViewProviderHandle HandleSelectorImpl(in MemoryViewConfig receiver)
    {
        return receiver.SharedConfig.GetReference()[receiver.IndividualConfig].Value.Item2;
    }

    public unsafe void Add<T, TMemoryView>(LowLevelChoice<object, nint> memorySource, MemoryViewProviderHandle<LowLevelChoice<object, nint>, T, TMemoryView> handle)
        where TMemoryView : IMemoryView<T>
    #if NET9_0_OR_GREATER
        ,allows ref struct
        where T : allows ref struct
    #endif
    {
        static void GetAbstractMemoryViewImpl(ref LowLevelChoice<object, nint> receiver, out TMemoryView memoryView)
        {
            receiver[receiver.IndividualConfig].Value.DangerousGetMemoryView<T, TMemoryView>(out memoryView);
        }
        static void GetMemoryViewImpl(ref MemoryViewConfig receiver, out TMemoryView memoryView)
        {
            receiver[receiver.IndividualConfig].Value.DangerousGetMemoryView<T, TMemoryView>(out memoryView);
        }

        ref var builder = ref SharedConfig.GetReference();
        int nextKey = builder.Length;
        MemoryViewConfig nextConfig = WithNewIndividualConfig(in nextKey);
        var abstractProvider = DangerousMemoryViewProviderReceiver<LowLevelChoice<object, nint>>.Create(in memorySource, )
        RawValue nextValue = new
        (
            DangerousMemoryViewProviderReceiver<MemoryViewConfig>.Create<T, TMemoryView>(in nextConfig, &HandleSelectorImpl),
            DangerousMemoryViewProviderHandle.Wrap
        )

        unsafe
        {
            builder.Add
            (
                new
                (
                    nextKey, 
                    DangerousMemoryViewProviderReceiver<MemoryViewConfig>.Create<T, TMemoryView>
                    (
                        WithNewIndividualConfig(in nextKey), 
                        new (&GetMemoryViewImpl)
                    )
                )
            );
        }
    }
}
