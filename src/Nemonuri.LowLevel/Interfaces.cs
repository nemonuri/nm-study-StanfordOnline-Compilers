
using System.Buffers;

namespace Nemonuri.LowLevel;

public interface IPackedTable<TKey, TValue> : 
    IMemoryOwner<LowLevelKeyValuePair<TKey, TValue>>
    where TKey : IEquatable<TKey>
{
}

public interface ISpanView<TView>
{
    int Length {get;}
    ref TView this[int index] {get;}
}
