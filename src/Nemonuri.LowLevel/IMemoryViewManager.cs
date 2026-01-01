
using CommunityToolkit.HighPerformance;

namespace Nemonuri.LowLevel;

public interface IMemoryViewManager<TKey, TReceiver> :
    IMemoryView<RawKeyValuePair<TKey, DangerousMemoryViewProviderReceiver<TReceiver>>>
{
    [UnscopedRef] ref readonly TKey GetIndividualKeyRef(ref TReceiver receiver);
}
