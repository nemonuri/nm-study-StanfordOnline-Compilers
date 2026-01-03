
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
    public TKey GetOrAddHandleComponent<T, TMemoryView>(MethodHandle<TReceiverComponent, TArgumentComponent, TMemoryView> handleComponent, out bool fresh)
        where TMemoryView : IMemoryView<T>;

    public LowLevelKeyValuePair<TKey, TProvider> GetOrBuildProvider(TKey receiver, TKey argument, TKey handle, out bool fresh);
}

public static class DangerousMemoryViewProviderBuilderTheory
{
    extension<TKey, TProvider, TReceiverComponent, TArgumentComponent>
    (IDangerousMemoryViewProviderBuilder<TKey, TProvider, TReceiverComponent, TArgumentComponent> self)
        where TProvider : IDangerousMemoryViewProvider
    {
        public LowLevelKeyValuePair<TKey, TProvider> AddAndBuild<T, TMemoryView>
        (
            TReceiverComponent receiverComponent, 
            TArgumentComponent argumentComponent, 
            MethodHandle<TReceiverComponent, TArgumentComponent, TMemoryView> handleComponent,
            TypeHint<T> th = default
        )
            where TMemoryView : IMemoryView<T>
        {
            var rk = self.GetOrAddReceiverComponent(receiverComponent, out _);
            var ak = self.GetOrAddArgumentComponent(argumentComponent, out _);
            var hk = self.GetOrAddHandleComponent<T, TMemoryView>(handleComponent, out _);

            return self.GetOrBuildProvider(rk, ak, hk, out _);
        }
    }
}