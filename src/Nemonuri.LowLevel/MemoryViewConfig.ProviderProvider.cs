
using CommunityToolkit.HighPerformance;
using Nemonuri.LowLevel.Abstractions;
using Nemonuri.LowLevel.Primitives;

namespace Nemonuri.LowLevel;

public readonly partial struct MemoryViewConfig
{
    public readonly struct ProviderProvider : IDangerousMemoryViewProvider
    {
        public readonly MemoryViewConfig MemoryViewConfig;
        public readonly int ProviderIndex;
        public readonly int ArgumentIndex;
        public readonly int MethodHandleIndex;

        public ProviderProvider(MemoryViewConfig memoryViewConfig, int providerIndex, int argumentIndex, int methodHandleIndex)
        {
            MemoryViewConfig = memoryViewConfig;
            ProviderIndex = providerIndex;
            ArgumentIndex = argumentIndex;
            MethodHandleIndex = methodHandleIndex;
        }

        [UnscopedRef]
        public ref TMemoryView DangerousGetMemoryView<T, TMemoryView>() where TMemoryView : IMemoryView<T> 
            => ref DangerousGetMemoryView<T, TMemoryView>(in this);

        private static ref TMemoryView DangerousGetMemoryView<T, TMemoryView>(in ProviderProvider pp)
            where TMemoryView : IMemoryView<T>
        {
            ref var shared = ref pp.MemoryViewConfig.SharedConfig.GetReference();
            ref var provider = ref shared.Providers[pp.ProviderIndex];
            ref var argument = ref shared.Arguments[pp.ArgumentIndex];
            ref var boxedMethodHandle = ref shared.GetMemoryViewHandles[pp.MethodHandleIndex];
            ref var methodHandle = ref boxedMethodHandle.DangerousUnbox<MethodHandle<ObjectOrPointer, ObjectOrPointer, TMemoryView>>();
            if (RuntimePointerTheory.IsUndefinedOrNullRef(ref methodHandle))
            {
                throw new InvalidCastException();
            }

            return ref methodHandle.InvokeMethod(ref provider, ref argument)!;
        }
    }

    public readonly record struct Individual
    {
        public int MemoryViewProviderKey {get;}

        internal Individual(int memoryViewProviderKey)
        {
            MemoryViewProviderKey = memoryViewProviderKey;
        }
    }

    public readonly struct Shared
    {
        public readonly ArrayViewBuilder<ObjectOrPointer> Providers;
        public readonly ArrayViewBuilder<ObjectOrPointer> Arguments;
        public readonly ArrayViewBuilder<TypedUnmanagedBox<nint>> GetMemoryViewHandles; // MethodHandle<ObjectOrPointer, ObjectOrPointer, TMemoryView>

        public readonly PackedTable<Individual, ProviderProvider>.Builder ProviderProviders;

        internal Shared
        (
            ArrayViewBuilder<ObjectOrPointer> providers, 
            ArrayViewBuilder<ObjectOrPointer> arguments, 
            ArrayViewBuilder<TypedUnmanagedBox<nint>> getMemoryViewHandles,
            PackedTable<Individual, ProviderProvider>.Builder providerProviders
        )
        {
            Providers = providers;
            Arguments = arguments;
            GetMemoryViewHandles = getMemoryViewHandles;
            ProviderProviders = providerProviders;
        }
    }
}
