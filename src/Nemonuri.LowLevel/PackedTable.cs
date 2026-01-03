
namespace Nemonuri.LowLevel;

public readonly struct PackedTableView<TKey, TValue> :
    IMemoryView<LowLevelKeyValuePair<TKey, TValue>>,
    IMaybeSupportsRawSpan<LowLevelKeyValuePair<TKey, TValue>>
    where TKey : IEquatable<TKey>
{
    private readonly ArrayView<LowLevelKeyValuePair<TKey, TValue>> _arrayView;

    internal PackedTableView(ArrayView<LowLevelKeyValuePair<TKey, TValue>> arrayView)
    {
        _arrayView = arrayView;
    }

    public int Length => _arrayView.Length;

    public ref LowLevelKeyValuePair<TKey, TValue> this[int index] => ref _arrayView[index];

    public bool SupportsRawSpan => true;

    public Span<LowLevelKeyValuePair<TKey, TValue>> AsSpan => _arrayView.AsSpan;
}

public readonly partial struct PackedTable<TKey, TValue> : 
    ILowLevelTable<TKey, TValue, PackedTableView<TKey, TValue>>
    where TKey : IEquatable<TKey>
{
    private readonly PackedTableView<TKey, TValue> _packedTableView;

    public PackedTable(PackedTableView<TKey, TValue> packedTableView)
    {
        _packedTableView = packedTableView;
    }

    public PackedTable(LowLevelKeyValuePair<TKey, TValue>[] memory) : this
    (
        new PackedTableView<TKey, TValue>(new ArrayView<LowLevelKeyValuePair<TKey, TValue>>(memory))
    )
    {}

    [UnscopedRef]
    public ref readonly PackedTableView<TKey, TValue> InvokeProvider() => ref _packedTableView;
}

