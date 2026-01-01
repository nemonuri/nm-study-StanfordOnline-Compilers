
namespace Nemonuri.LowLevel;

public readonly partial struct PackedTable<TKey, TValue> where TKey : IEquatable<TKey>
{
    public static Builder CreateBuilder(int initialCapacity) => new(initialCapacity);

    public readonly struct Builder : 
        IMemoryView<LowLevelKeyValuePair<TKey, TValue>>, 
        IMaybeSupportsRawSpan<LowLevelKeyValuePair<TKey, TValue>>,
        IBuilder<LowLevelKeyValuePair<TKey, TValue>, PackedTableView<TKey, TValue>>
    {
        private readonly ArrayViewBuilder<LowLevelKeyValuePair<TKey, TValue>> _builder;

        internal Builder(int initialCapacity)
        {
            _builder = new(initialCapacity);
        }

        public PackedTable<TKey, TValue> ToPackedTable()
        {
            return new(_builder.ToArray());
        }

        public int Length => _builder.Length;

        public ref LowLevelKeyValuePair<TKey, TValue> this[int index] => ref _builder[index];

        public bool SupportsRawSpan => _builder.SupportsRawSpan;

        public Span<LowLevelKeyValuePair<TKey, TValue>> AsSpan => _builder.AsSpan;

        public void Add(in LowLevelKeyValuePair<TKey, TValue> source) => _builder.Add(in source);

        public PackedTableView<TKey, TValue> Build() => new(_builder.Build());
    }
}
