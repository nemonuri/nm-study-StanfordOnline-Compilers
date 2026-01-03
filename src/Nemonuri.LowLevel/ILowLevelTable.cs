
namespace Nemonuri.LowLevel;

public interface ILowLevelTable<TKey, TValue, TMemoryView> :
    IMemoryViewProvider<LowLevelKeyValuePair<TKey, TValue>, TMemoryView>
    where TMemoryView : IMemoryView<LowLevelKeyValuePair<TKey, TValue>>
#if NET9_0_OR_GREATER
    ,allows ref struct
#endif
    where TKey : IEquatable<TKey>
{
}

public struct LowLevelTableReceiver<TReceiver, TKey, TValue> :
    ILowLevelTable<TKey, TValue, MemoryViewReceiver<TReceiver, LowLevelKeyValuePair<TKey, TValue>>>
    where TKey : IEquatable<TKey>
{
    private MemoryViewProviderReceiver<TReceiver, LowLevelKeyValuePair<TKey, TValue>, MemoryViewReceiver<TReceiver, LowLevelKeyValuePair<TKey, TValue>>> _provider;

    public LowLevelTableReceiver(MemoryViewProviderReceiver<TReceiver, LowLevelKeyValuePair<TKey, TValue>, MemoryViewReceiver<TReceiver, LowLevelKeyValuePair<TKey, TValue>>> provider)
    {
        _provider = provider;
    }

    public void GetMemoryView(scoped ref MemoryViewReceiver<TReceiver, LowLevelKeyValuePair<TKey, TValue>> memoryView) => _provider.GetMemoryView(ref memoryView);
}
