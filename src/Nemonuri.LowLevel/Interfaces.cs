
using System.Buffers;

namespace Nemonuri.LowLevel;

public interface IMemoryViewOwner<TView>
{
}

public interface ILowLevelTable<TKey, TValue> :
    IMemoryViewOwner<LowLevelKeyValuePair<TKey, TValue>>
{
}

public interface IPackedTable<TKey, TValue> :
    IMemoryOwner<LowLevelKeyValuePair<TKey, TValue>>
    where TKey : IEquatable<TKey>
{
}
