
namespace Nemonuri.LowLevel;

public interface IMemoryViewManager<TKey, TProvider> :
    IMemoryView<LowLevelKeyValuePair<TKey, DuckTypedProperty<TProvider, TypedUnmanagedBox<nint>>>>
{
    //[UnscopedRef] ref readonly TKey GetIndividualKeyRef(ref TReceiver receiver);
}
