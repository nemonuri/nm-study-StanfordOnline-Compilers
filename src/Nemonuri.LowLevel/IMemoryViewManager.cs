
using CommunityToolkit.HighPerformance;

namespace Nemonuri.LowLevel;

public interface IMemoryViewManager<TKey, TReceiver> :
    IMemoryView<RawKeyValuePair<TKey, DangerousMemoryViewProviderReceiver<TReceiver>>>
{
    [UnscopedRef] ref readonly TKey GetIndividualKeyRef(ref TReceiver receiver);
}

public readonly struct MemoryViewConfig : 
    IMemoryViewManager<int, MemoryViewConfig>,
    IConfig<Box<PackedTable<int, DangerousMemoryViewProviderReceiver<MemoryViewConfig>>.Builder>, int, MemoryViewConfig>
{
    public Box<PackedTable<int, DangerousMemoryViewProviderReceiver<MemoryViewConfig>>.Builder> SharedConfig {get;}
    private readonly int _key;

    internal MemoryViewConfig
    (
        Box<PackedTable<int, DangerousMemoryViewProviderReceiver<MemoryViewConfig>>.Builder> shardConfig,
        int key
    )
    {
        SharedConfig = shardConfig ?? PackedTable<int, DangerousMemoryViewProviderReceiver<MemoryViewConfig>>.CreateBuilder(0);
        _key = key;
    }

    public ref readonly int GetIndividualKeyRef(ref MemoryViewConfig receiver) => ref receiver.IndividualConfig;

    public int Length => SharedConfig.GetReference().Length;

    public ref RawKeyValuePair<int, DangerousMemoryViewProviderReceiver<MemoryViewConfig>> this[int index] => ref SharedConfig.GetReference()[index];

    [UnscopedRef]
    public ref readonly int IndividualConfig => ref _key;

    public MemoryViewConfig WithNewIndividualConfig(in int individual) => new(SharedConfig, individual);

    public void Add<T, TMemoryView>(TMemoryView memoryView)
        where TMemoryView : IMemoryView<T>
    #if NET9_0_OR_GREATER
        ,allows ref struct
        where T : allows ref struct
    #endif
    {
        static void GetMemoryViewImpl(ref MemoryViewConfig receiver, out TMemoryView memoryView)
        {
            receiver[receiver.IndividualConfig].Value.DangerousGetMemoryView<T, TMemoryView>(out memoryView);
        }

        ref var builder = ref SharedConfig.GetReference();
        int nextKey = builder.Length;

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
