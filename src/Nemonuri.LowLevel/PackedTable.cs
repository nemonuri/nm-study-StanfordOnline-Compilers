
using System.Collections.Immutable;

namespace Nemonuri.LowLevel;

public readonly struct PackedTableView<TKey, TValue> :
    IMemoryView<LowLevelKeyValuePair<TKey, TValue>>
    where TKey : IEquatable<TKey>
{
    private readonly ArrayView<LowLevelKeyValuePair<TKey, TValue>> _arrayView;

    internal PackedTableView(ArrayView<LowLevelKeyValuePair<TKey, TValue>> arrayView)
    {
        _arrayView = arrayView;
    }

    public int Length => _arrayView.Length;

    public ref LowLevelKeyValuePair<TKey, TValue> this[int index] => ref _arrayView[index];
}

public readonly partial struct PackedTable<TKey, TValue> : 
    ILowLevelTable<TKey, TValue, PackedTableView<TKey, TValue>>
    where TKey : IEquatable<TKey>
{
    public PackedTable(LowLevelKeyValuePair<TKey, TValue>[] memory)
    {
        Memory = memory;
    }

    public LowLevelKeyValuePair<TKey, TValue>[] Memory {get;}

    public void GetMemoryView(scoped ref PackedTableView<TKey, TValue> memoryView)
    {
        unsafe
        {
            memoryView = new(new(Memory));
        }
    }
}

