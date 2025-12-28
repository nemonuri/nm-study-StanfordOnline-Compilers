
using System.Collections.Immutable;

namespace Nemonuri.LowLevel;

public readonly partial struct PackedTable<TKey, TValue> where TKey : IEquatable<TKey>
{
    public class Builder : IMemoryView<LowLevelKeyValuePair<TKey, TValue>>
    {
        private readonly ImmutableArray<LowLevelKeyValuePair<TKey, TValue>>.Builder _builder;

        internal Builder()
        {
            _builder = ImmutableArray.CreateBuilder<LowLevelKeyValuePair<TKey, TValue>>();
        }

        public int Length => _builder.Count;

        public ref LowLevelKeyValuePair<TKey, TValue> this[int index] => ref Unsafe.AsRef(in _builder.ItemRef(index));

        public void Add(TKey key, TValue value) => _builder.Add(new(key, value));
    }
}

