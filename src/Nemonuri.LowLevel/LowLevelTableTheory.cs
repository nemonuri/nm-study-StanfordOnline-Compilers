
namespace Nemonuri.LowLevel;

public static class LowLevelTableTheory
{
    extension<TKey, TValue, TMemoryView, TTable>
    (scoped in 
        TheoryBox
        <(TKey, TValue, TMemoryView), TTable> 
        theory)
        where TKey : IEquatable<TKey>
        where TMemoryView : IMemoryView<RawKeyValuePair<TKey, TValue>>
        where TTable : ILowLevelTable<TKey, TValue, TMemoryView>
    {
        public static
        TheoryBox
        <(TKey, TValue, TMemoryView), TTable>
        Theorize
        (ref readonly TTable source)
        =>
        TheoryBox.Box
        <(TKey, TValue, TMemoryView), TTable>
        (in source);

        public TMemoryView MemoryView
        {
            get
            {
                TMemoryView result = default!;
                theory.Self.GetMemoryView(ref result);
                return result;
            }
        }

        public int Length => theory.MemoryView.Length;

        public bool TryGetValue(in TKey key, out TValue? resultValue)
        {
            var boxedMemoryView = MemoryViewTheory.Theorize<RawKeyValuePair<TKey, TValue>, TMemoryView>(theory.MemoryView);
            if (!boxedMemoryView.TryGetEntry(in key, out var resultEntry, out _)) 
            {
                resultValue = default;
                return false;
            }

            resultValue = resultEntry.Value;
            return true;
        }
    }
}
