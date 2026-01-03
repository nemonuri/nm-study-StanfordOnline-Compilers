
using Nemonuri.LowLevel.Primitives;

namespace Nemonuri.LowLevel;

public partial class MemoryViewManager<TBuilderReceiver, TBuilderArgument>
{
    public readonly struct Provider : IDangerousMemoryViewProvider
    {
        public readonly MemoryViewManager<TBuilderReceiver, TBuilderArgument> MemoryViewManager;
        public readonly int ProviderIndex;
        public readonly int ArgumentIndex;
        public readonly int MethodHandleIndex;

        public Provider(MemoryViewManager<TBuilderReceiver, TBuilderArgument> memoryViewManager, int providerIndex, int argumentIndex, int methodHandleIndex)
        {
            MemoryViewManager = memoryViewManager;
            ProviderIndex = providerIndex;
            ArgumentIndex = argumentIndex;
            MethodHandleIndex = methodHandleIndex;
        }

        [UnscopedRef]
        public ref TMemoryView DangerousGetMemoryView<T, TMemoryView>() where TMemoryView : IMemoryView<T> 
            => ref DangerousGetMemoryView<T, TMemoryView>(in this);

        private static ref TMemoryView DangerousGetMemoryView<T, TMemoryView>(in Provider pp)
            where TMemoryView : IMemoryView<T>
        {
            ref readonly var shared = ref pp.MemoryViewManager.SharedState;
            ref var provider = ref shared.Providers[pp.ProviderIndex];
            ref var argument = ref shared.Arguments[pp.ArgumentIndex];
            ref var boxedMethodHandle = ref shared.GetMemoryViewHandles[pp.MethodHandleIndex];
            ref var methodHandle = ref boxedMethodHandle.DangerousUnbox<MethodHandle<TBuilderReceiver, TBuilderArgument, TMemoryView>>();
            if (RuntimePointerTheory.IsUndefinedOrNullRef(ref methodHandle))
            {
                throw new InvalidCastException();
            }

            return ref methodHandle.InvokeMethod(ref provider, ref argument)!;
        }
    }

    public readonly struct Shared
    {
        public readonly ArrayViewBuilder<TBuilderReceiver> Providers;
        public readonly ArrayViewBuilder<TBuilderArgument> Arguments;
        public readonly ArrayViewBuilder<TypedUnmanagedBox<nint>> GetMemoryViewHandles; // MethodHandle<TAbstractProvider, TAbstractArgument, TMemoryView>

        public readonly PackedTable<int, Provider>.Builder ProviderProviders;

        internal Shared
        (
            ArrayViewBuilder<TBuilderReceiver> providers, 
            ArrayViewBuilder<TBuilderArgument> arguments, 
            ArrayViewBuilder<TypedUnmanagedBox<nint>> getMemoryViewHandles,
            PackedTable<int, Provider>.Builder providerProviders
        )
        {
            Providers = providers;
            Arguments = arguments;
            GetMemoryViewHandles = getMemoryViewHandles;
            ProviderProviders = providerProviders;
        }
    }
}
