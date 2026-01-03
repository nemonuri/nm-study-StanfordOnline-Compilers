
namespace Nemonuri.LowLevel;

public interface IMemoryViewManager<TKey, TProvider> :
    IMemoryView<LowLevelKeyValuePair<TKey, TProvider>>
    where TProvider : IDangerousProviderArity2
{
    //[UnscopedRef] ref readonly TKey GetIndividualKeyRef(ref TReceiver receiver);
}
