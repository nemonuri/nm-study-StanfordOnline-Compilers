
namespace Nemonuri.LowLevel;

public interface IMemoryViewProvider<TView, TMemoryView>
    where TMemoryView : IMemoryView<TView>
#if NET9_0_OR_GREATER
    ,allows ref struct
#endif
{
    void GetMemoryView(scoped ref TMemoryView memoryView);
}

public interface ILowLevelTable<TKey, TValue, TMemoryView> :
    IMemoryViewProvider<LowLevelKeyValuePair<TKey, TValue>, TMemoryView>
    where TMemoryView : IMemoryView<LowLevelKeyValuePair<TKey, TValue>>
#if NET9_0_OR_GREATER
    ,allows ref struct
#endif
    where TKey : IEquatable<TKey>
{
}
