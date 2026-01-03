
using CommunityToolkit.HighPerformance;
using Nemonuri.LowLevel.Abstractions;
using Nemonuri.LowLevel.Primitives;

namespace Nemonuri.LowLevel;

public readonly partial struct MemoryViewConfig
{
    public readonly struct ProviderProvider : IDangerousProviderArity2
    {
        public readonly MemoryViewConfig MemoryViewConfig;

        internal ProviderProvider(MemoryViewConfig memoryViewConfig)
        {
            MemoryViewConfig = memoryViewConfig;
            ProviderIndex = ArgumentIndex = MethodHandleIndex = -1;
        }

        public int ProviderIndex {get;internal init;}
        public int ArgumentIndex {get;internal init;}
        public int MethodHandleIndex {get;internal init;}

        public ref TMemoryView DangerousGet<T, TMemoryView>()
            where TMemoryView : IMemoryView<T>
        {
            throw new NotImplementedException();
        }


        private static ref TMemoryView DangerousGetMemoryView<T, TMemoryView>(ref ProviderProvider pp)
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

        public readonly PackedTable<Individual, DuckTypedProperty<ProviderProvider, TypedUnmanagedBox<nint>>>.Builder ProviderProviders;

        internal Shared
        (
            ArrayViewBuilder<ObjectOrPointer> providers, 
            ArrayViewBuilder<ObjectOrPointer> arguments, 
            ArrayViewBuilder<TypedUnmanagedBox<nint>> getMemoryViewHandles,
            PackedTable<Individual, DuckTypedProperty<ProviderProvider, TypedUnmanagedBox<nint>>>.Builder providerProviders
        )
        {
            Providers = providers;
            Arguments = arguments;
            GetMemoryViewHandles = getMemoryViewHandles;
            ProviderProviders = providerProviders;
        }
    }
}
