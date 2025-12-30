
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

public struct AbstractLowLevelTable<THandler, TKey, TValue> :
    ILowLevelTable<TKey, TValue, MemoryViewReceiver<THandler, LowLevelKeyValuePair<TKey, TValue>>>
    where TKey : IEquatable<TKey>
{
    private MemoryViewProviderReceiver<THandler, LowLevelKeyValuePair<TKey, TValue>, MemoryViewReceiver<THandler, LowLevelKeyValuePair<TKey, TValue>>> _provider;

    public AbstractLowLevelTable(MemoryViewProviderReceiver<THandler, LowLevelKeyValuePair<TKey, TValue>, MemoryViewReceiver<THandler, LowLevelKeyValuePair<TKey, TValue>>> provider)
    {
        _provider = provider;
    }

    public void GetMemoryView(scoped ref MemoryViewReceiver<THandler, LowLevelKeyValuePair<TKey, TValue>> memoryView) => _provider.GetMemoryView(ref memoryView);
}
