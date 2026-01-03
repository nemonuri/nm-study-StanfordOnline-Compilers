
namespace Nemonuri.LowLevel;

public interface IDangerousMemoryViewProvider
{
    [UnscopedRef] ref TMemoryView DangerousGetMemoryView<T, TMemoryView>() where TMemoryView : IMemoryView<T> ;
}

public interface IAddableDangerousMemoryViewProviderTable<TKey, TProvider, TReceiverComponent, TArgumentComponent> :
    IMemoryView<LowLevelKeyValuePair<TKey, TProvider>>
    where TProvider : IDangerousMemoryViewProvider
{
    public TKey AddReceiverComponent(TReceiverComponent receiverComponent);
    public TKey AddArgumentComponent(TArgumentComponent argumentComponent);
    public TKey AddHandleComponent<T, TMemoryView>(MethodHandle<TReceiverComponent, TArgumentComponent, TMemoryView> handleComponent)
        where TMemoryView : IMemoryView<T>;

    public LowLevelKeyValuePair<TKey, TProvider> BuildProvider(TKey receiver, TKey argument, TKey handle);

    public void Add<T, TMemoryView>
    (
        TReceiverComponent receiverComponent, 
        TArgumentComponent argumentComponent, 
        MethodHandle<TReceiverComponent, TArgumentComponent, TMemoryView> handleComponent
    ) where TMemoryView : IMemoryView<T> ;
}

public static class AddableDangerousMemoryViewProviderTableTheory
{
    extension<TKey, TProvider, TReceiverComponent, TArgumentComponent>
    (IAddableDangerousMemoryViewProviderTable<TKey, TProvider, TReceiverComponent, TArgumentComponent> self)
        where TProvider : IDangerousMemoryViewProvider
    {
        public LowLevelKeyValuePair<TKey, TProvider> AddAndBuild<T, TMemoryView>(MethodHandle<TReceiverComponent, TArgumentComponent, TMemoryView> handleComponent, TypeHint<T> th = default)
        {
            var rk = self.AddReceiverComponent()
        }
    }
}