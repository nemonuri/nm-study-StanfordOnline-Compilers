
namespace Nemonuri.LowLevel;

public interface ILowLevelTable<TKey, TValue, TMemoryView> :
    IMemoryViewProvider<RawKeyValuePair<TKey, TValue>, TMemoryView>
    where TMemoryView : IMemoryView<RawKeyValuePair<TKey, TValue>>
#if NET9_0_OR_GREATER
    ,allows ref struct
#endif
    where TKey : IEquatable<TKey>
{
}

public struct LowLevelTableReceiver<TReceiver, TKey, TValue> :
    ILowLevelTable<TKey, TValue, MemoryViewReceiver<TReceiver, RawKeyValuePair<TKey, TValue>>>
    where TKey : IEquatable<TKey>
{
    private MemoryViewProviderReceiver<TReceiver, RawKeyValuePair<TKey, TValue>, MemoryViewReceiver<TReceiver, RawKeyValuePair<TKey, TValue>>> _provider;

    public LowLevelTableReceiver(MemoryViewProviderReceiver<TReceiver, RawKeyValuePair<TKey, TValue>, MemoryViewReceiver<TReceiver, RawKeyValuePair<TKey, TValue>>> provider)
    {
        _provider = provider;
    }

    public void GetMemoryView(scoped ref MemoryViewReceiver<TReceiver, RawKeyValuePair<TKey, TValue>> memoryView) => _provider.GetMemoryView(ref memoryView);
}
