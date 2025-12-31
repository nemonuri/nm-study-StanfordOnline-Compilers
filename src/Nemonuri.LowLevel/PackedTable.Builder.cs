
namespace Nemonuri.LowLevel;

public readonly partial struct PackedTable<TKey, TValue> where TKey : IEquatable<TKey>
{
    public static Builder CreateBuilder(int initialCapacity = 4) => new(initialCapacity);

    public readonly struct Builder : 
        IMemoryView<LowLevelKeyValuePair<TKey, TValue>>, 
        IMaybeSupportsRawSpan<LowLevelKeyValuePair<TKey, TValue>>
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

        public void Add(TKey key, TValue value) => _builder.Add(new(key, value));

        public bool SupportsRawSpan => _builder.SupportsRawSpan;

        public Span<LowLevelKeyValuePair<TKey, TValue>> AsSpan => _builder.AsSpan;
    }
}
