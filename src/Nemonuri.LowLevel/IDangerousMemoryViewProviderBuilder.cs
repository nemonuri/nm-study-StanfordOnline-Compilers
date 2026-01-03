
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
    public TKey AddReceiverComponent(TReceiverComponent receiverComponent);
    public TKey AddArgumentComponent(TArgumentComponent argumentComponent);
    public TKey AddHandleComponent<T, TMemoryView>(MethodHandle<TReceiverComponent, TArgumentComponent, TMemoryView> handleComponent)
        where TMemoryView : IMemoryView<T>;

    public LowLevelKeyValuePair<TKey, TProvider> BuildProvider(TKey receiver, TKey argument, TKey handle);
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
            var rk = self.AddReceiverComponent(receiverComponent);
            var ak = self.AddArgumentComponent(argumentComponent);
            var hk = self.AddHandleComponent<T, TMemoryView>(handleComponent);

            return self.BuildProvider(rk, ak, hk);
        }
    }
}