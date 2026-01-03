
namespace Nemonuri.LowLevel;

public interface IMemoryViewManager<TKey, TProvider> :
    IMemoryView<LowLevelKeyValuePair<TKey, TProvider>>
    where TProvider : IDangerousMemoryViewProvider
{
    //[UnscopedRef] ref readonly TKey GetIndividualKeyRef(ref TReceiver receiver);
}

public interface IDangerousMemoryViewProvider
{
    [UnscopedRef] ref TMemoryView DangerousGetMemoryView<T, TMemoryView>() where TMemoryView : IMemoryView<T> ;
}