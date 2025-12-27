
using System.Buffers;

namespace Nemonuri.LowLevel;

public interface IPackedTable<TKey, TValue> :
    IMemoryOwner<LowLevelKeyValuePair<TKey, TValue>>
    where TKey : IEquatable<TKey>
{
}

public interface ISpanViewProvider<TView>
{
}

public interface ISpanViewOwner<T, TView> : ISpanViewProvider<TView>
{
    void GetSpanView(scoped ref SpanView<T, TView> spanView);
}
