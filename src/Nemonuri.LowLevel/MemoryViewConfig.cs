
using CommunityToolkit.HighPerformance;
using Nemonuri.LowLevel.Abstractions;
using Nemonuri.LowLevel.Primitives;

namespace Nemonuri.LowLevel;

public readonly partial struct MemoryViewConfig : 
    IMemoryViewManager<MemoryViewConfig.Individual, MemoryViewConfig.ProviderProvider>,
    IConfig<Box<MemoryViewConfig.Shared>, MemoryViewConfig.Individual, MemoryViewConfig>
{
    private readonly Individual _individualConfig;
    public Box<Shared> SharedConfig {get;}

    internal MemoryViewConfig(Box<Shared> shared, Individual individual)
    {
        SharedConfig = shared;
        _individualConfig = individual;
    }

    public int Length => SharedConfig.GetReference().ProviderProviders.Length;

    public ref LowLevelKeyValuePair<Individual, DuckTypedProperty<ProviderProvider, TypedUnmanagedBox<nint>>> this[int index] => ref SharedConfig.GetReference().ProviderProviders[index];

    [UnscopedRef]
    public ref readonly Individual IndividualConfig => ref _individualConfig;

    public MemoryViewConfig WithNewIndividualConfig(in Individual individual) => new(SharedConfig, individual);

    public static MemoryViewConfig CreateNew()
    {
        return new (new Shared(new(4),new(4),new(4),new(4)), new(-1));
    }

    public bool Add<T, TMemoryView>
    (
        ObjectOrPointer memoryViewProvider, 
        ObjectOrPointer memoryViewArgument, 
        MethodHandle<ObjectOrPointer, ObjectOrPointer, TMemoryView> getMemoryViewHandle
    )
        where TMemoryView : IMemoryView<T>
    {
        ref var shared = ref SharedConfig.GetReference();

        // Note
        // - MemoryViewConfig.Provider 는, memoryViewProviderProvider 가 되는 것인가...;;
        // - 뭔가 구현하려다 보면, ReferenceReference 나, ProviderProvider 같은 게 꼭 생긴 단 말이지;;
        // - ...그래. 그냥 MemoryViewConfig.ProviderProvider 라고, 대놓고 지르자!


    }



}
