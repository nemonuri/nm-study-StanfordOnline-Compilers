
using CommunityToolkit.HighPerformance;
using RawValue =
        Nemonuri.LowLevel.SequentialLayoutValueTuple<
            Nemonuri.LowLevel.DangerousMemoryViewProviderReceiver<Nemonuri.LowLevel.MemoryViewConfig>, 
            Nemonuri.LowLevel.DangerousMemoryViewProviderHandle,
            Nemonuri.LowLevel.DangerousMemoryViewProviderReceiver<Nemonuri.LowLevel.LowLevelChoice<object, nint>>
        >;
using RawEntry = 
    Nemonuri.LowLevel.LowLevelKeyValuePair<
        int, 
        Nemonuri.LowLevel.SequentialLayoutValueTuple<
            Nemonuri.LowLevel.DangerousMemoryViewProviderReceiver<Nemonuri.LowLevel.MemoryViewConfig>, 
            Nemonuri.LowLevel.DangerousMemoryViewProviderHandle,
            Nemonuri.LowLevel.DangerousMemoryViewProviderReceiver<Nemonuri.LowLevel.LowLevelChoice<object, nint>>
        >
    >;
using Nemonuri.LowLevel.Abstractions;

namespace Nemonuri.LowLevel;

public readonly struct MemoryViewConfig : 
    IMemoryViewManager<MemoryViewConfig.Individual, MemoryViewConfig.Provider>,
    IConfig<Box<MemoryViewConfig.Shared>, MemoryViewConfig.Individual, MemoryViewConfig>
{
    private readonly Individual _individualConfig;
    public Box<Shared> SharedConfig {get;}

    internal MemoryViewConfig(Box<Shared> shared, Individual individual)
    {
        SharedConfig = shared;
        _individualConfig = individual;
    }

    public int Length => SharedConfig.GetReference().MemoryViewManager.Length;

    public ref LowLevelKeyValuePair<Individual, DuckTypedProperty<Provider, TypedUnmanagedBox<nint>>> this[int index] => ref SharedConfig.GetReference().MemoryViewManager[index];

    [UnscopedRef]
    public ref readonly Individual IndividualConfig => ref _individualConfig;

    public MemoryViewConfig WithNewIndividualConfig(in Individual individual) => new(SharedConfig, individual);



    public static MemoryViewConfig CreateNew()
    {
        Individual individual = new(0, 0, 0)
    }


    public readonly record struct Individual
    {
        public int MemoryViewProviderKey {get;}

        internal Individual(int memoryViewProviderKey)
        {
            MemoryViewProviderKey = memoryViewProviderKey;
        }
    }

    public readonly struct Provider
    {
        public readonly MemoryViewConfig MemoryViewConfig;

        internal Provider(MemoryViewConfig memoryViewConfig)
        {
            MemoryViewConfig = memoryViewConfig;
            ReceiverIndex = ArgumentIndex = MethodHandleIndex = -1;
        }

        public int ReceiverIndex {get;internal init;}
        public int ArgumentIndex {get;internal init;}
        public int MethodHandleIndex {get;internal init;}
    }

    public readonly struct Shared
    {
        public readonly ArrayViewBuilder<ObjectOrPointer> Receivers;
        public readonly ArrayViewBuilder<ObjectOrPointer> Arguments;
        public readonly ArrayViewBuilder<TypedUnmanagedBox<nint>> MethodHandles; // MethodHandle<ObjectOrPointer, ObjectOrPointer, TMemoryView>

        public readonly PackedTable<Individual, DuckTypedProperty<Provider, TypedUnmanagedBox<nint>>>.Builder MemoryViewManager;

        internal Shared
        (
            ArrayViewBuilder<ObjectOrPointer> receivers, 
            ArrayViewBuilder<ObjectOrPointer> arguments, 
            ArrayViewBuilder<TypedUnmanagedBox<nint>> methodHandles,
            PackedTable<Individual, DuckTypedProperty<Provider, TypedUnmanagedBox<nint>>>.Builder memoryViewManager
        )
        {
            Receivers = receivers;
            Arguments = arguments;
            MethodHandles = methodHandles;
            MemoryViewManager = memoryViewManager;
        }
    }


}
