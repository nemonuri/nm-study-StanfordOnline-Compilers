
namespace Nemonuri.LowLevel;

public readonly struct PackedTableView<TKey, TValue> :
    IMemoryView<RawKeyValuePair<TKey, TValue>>,
    IMaybeSupportsRawSpan<RawKeyValuePair<TKey, TValue>>
    where TKey : IEquatable<TKey>
{
    private readonly ArrayView<RawKeyValuePair<TKey, TValue>> _arrayView;

    internal PackedTableView(ArrayView<RawKeyValuePair<TKey, TValue>> arrayView)
    {
        _arrayView = arrayView;
    }

    public int Length => _arrayView.Length;

    public ref RawKeyValuePair<TKey, TValue> this[int index] => ref _arrayView[index];

    public bool SupportsRawSpan => true;

    public Span<RawKeyValuePair<TKey, TValue>> AsSpan => _arrayView.AsSpan;
}

public readonly partial struct PackedTable<TKey, TValue> : 
    ILowLevelTable<TKey, TValue, PackedTableView<TKey, TValue>>
    where TKey : IEquatable<TKey>
{
    public PackedTable(RawKeyValuePair<TKey, TValue>[] memory)
    {
        Memory = memory;
    }

    public RawKeyValuePair<TKey, TValue>[] Memory {get;}

    public void GetMemoryView(scoped ref PackedTableView<TKey, TValue> memoryView)
    {
        unsafe
        {
            memoryView = new(new(Memory));
        }
    }
}

