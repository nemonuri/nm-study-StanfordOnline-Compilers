
using Nemonuri.LowLevel.DuckTyping;

namespace Nemonuri.LowLevel;

public interface IDangerousMemoryViewProvider
{
    [UnscopedRef] ref TMemoryView DangerousGetMemoryView<T, TMemoryView>() where TMemoryView : IMemoryView<T> ;
}

public interface IDangerousMemoryViewProviderBuilder<TKey, TProvider, TReceiverComponent, TArgumentComponent> :
    IMemoryView<LowLevelKeyValuePair<TKey, TProvider>>
    where TProvider : IDangerousMemoryViewProvider
{
    public TKey GetOrAddReceiverComponent(TReceiverComponent receiverComponent, out bool fresh);
    public TKey GetOrAddArgumentComponent(TArgumentComponent argumentComponent, out bool fresh);

    public TKey GetOrAddHandleComponent(TypedUnmanagedBox<nint> handleComponent, out bool fresh);
    //public TKey GetOrAddHandleComponent<T, TMemoryView>(MethodHandle<TReceiverComponent, TArgumentComponent, TMemoryView> handleComponent, out bool fresh)
    //    where TMemoryView : IMemoryView<T>;

    public LowLevelKeyValuePair<TKey, TProvider> GetOrBuildProvider(TKey receiver, TKey argument, TKey handle, out bool fresh);
}

public static class DangerousMemoryViewProviderBuilderTheory
{
    extension<TKey, TProvider, TReceiverComponent, TArgumentComponent>
    (IDangerousMemoryViewProviderBuilder<TKey, TProvider, TReceiverComponent, TArgumentComponent> self)
        where TProvider : IDangerousMemoryViewProvider
    {
        public LowLevelKeyValuePair<TKey, TProvider> AddAndBuild
        (
            TReceiverComponent receiverComponent, 
            TArgumentComponent argumentComponent, 
            TypedUnmanagedBox<nint> handleComponent,
            out AddAndBuildFresh fresh
        )
        {
            fresh = default;
            var rk = self.GetOrAddReceiverComponent(receiverComponent, out fresh.Receiver);
            var ak = self.GetOrAddArgumentComponent(argumentComponent, out fresh.Argument);
            var hk = self.GetOrAddHandleComponent(handleComponent, out fresh.Handle);

            return self.GetOrBuildProvider(rk, ak, hk, out fresh.Build);
        }


    }

    public record struct AddAndBuildFresh
    {
        public bool Receiver, Argument, Handle, Build;

        public AddAndBuildFresh(bool receiver, bool argument, bool handle, bool build)
        {
            Receiver = receiver;
            Argument = argument;
            Handle = handle;
            Build = build;
        }
    }
}