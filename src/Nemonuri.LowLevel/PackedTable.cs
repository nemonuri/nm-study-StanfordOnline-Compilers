
namespace Nemonuri.LowLevel;

public readonly struct PackedTable<TKey, TValue> : IPackedTable<TKey, TValue>
    where TKey : IEquatable<TKey>
{
    public PackedTable(Memory<LowLevelKeyValuePair<TKey, TValue>> memory)
    {
        Memory = memory;
    }

    public Memory<LowLevelKeyValuePair<TKey, TValue>> Memory {get;}

    public void Dispose()
    {
    }
}
