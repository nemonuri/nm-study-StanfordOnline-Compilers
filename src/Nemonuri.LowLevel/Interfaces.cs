
using System.Buffers;

namespace Nemonuri.LowLevel;

public interface ISpanViewProvider<TView>
{
}

public interface ISpanViewOwner<T, TView> : ISpanViewProvider<TView>
{
    void GetSpanView(scoped ref SpanView<T, TView> spanView);
}

public interface ISpanView<TView>
{
    int Length {get;}

    ref TView this[int index] {get;}
}

public interface IPackedTable<TKey, TValue> :
    IMemoryOwner<LowLevelKeyValuePair<TKey, TValue>>
    where TKey : IEquatable<TKey>
{
}
