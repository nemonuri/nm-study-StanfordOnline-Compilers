
using static Nemonuri.LowLevel.Extensions.SpanExtensions;

namespace Nemonuri.LowLevel;

public static class PackedTableTheory
{
    extension<TKey, TValue, TPackedTable>
    (scoped in 
        TheoryBox
        <(TKey, TValue), TPackedTable> 
        theory)
        where TKey : IEquatable<TKey>
        where TPackedTable : IPackedTable<TKey, TValue>
    {
        public static
        TheoryBox
        <(TKey, TValue), TPackedTable>
        Theorize
        (ref readonly TPackedTable source)
        =>
        TheoryBox.Box
        <(TKey, TValue), TPackedTable>
        (in source);

        public Span<LowLevelKeyValuePair<TKey, TValue>> AsSpan => theory.Self.Memory.Span;

        public int Length => theory.Self.Memory.Length;

        public bool TryGetValue(in TKey key, [NotNullWhen(true)] out TValue? resultValue)
        {
            if (!theory.AsSpan.TryGetEntry(in key, out var resultEntry, out _)) 
            {
                resultValue = default;
                return false;
            }

            resultValue = resultEntry.Value!;
            return true;
        }
    }
}
