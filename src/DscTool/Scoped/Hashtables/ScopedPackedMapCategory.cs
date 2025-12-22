using DscTool.Infrastructure;

namespace DscTool.Scoped.Hashtables;

public readonly struct ScopedPackedMapCategory<T, TCondition, TCategory, TKey> :
    IScopedCategory<PackedMap<TKey, T>, PackedMap<TKey, TCondition>>
    where TCategory : IScopedCategory<T, TCondition>
    where TKey : IEquatable<TKey>
{
    private readonly PackedMap<TKey, TCategory> _categoryMap;

    public ScopedPackedMapCategory(PackedMap<TKey, TCategory> categoryMap)
    {
        _categoryMap = categoryMap;
    }

    public bool Equals(PackedMap<TKey, T> xMap, PackedMap<TKey, T> yMap)
    {
        int length = xMap.Length;
        if (length != yMap.Length) {return false;}
        if (!_categoryMap.Fallback.Equals(xMap.Fallback, yMap.Fallback)) {return false;}

        foreach (var xEntry in xMap.AsSpan)
        {
            var xKey = xEntry.Key;

            RawKeyValuePair<OptionalKey<TKey>, T> yEntry = yMap.GetEntryOrFallback(xKey);
            if (!yEntry.Key.IsSome) {return false;}
            if (!yEntry.Key.GetSome().Equals(xKey)) {return false;}
            if (!_categoryMap.GetEntryOrFallback(xKey).Value.Equals(xEntry.Value, yEntry.Value)) {return false;}
        }

        return true;
    }

    public int GetHashCode(PackedMap<TKey, T> xMap)
    {
        int hashCode = _categoryMap.Fallback.GetHashCode(xMap.Fallback);
        foreach (var entry in xMap.AsSpan)
        {
            hashCode ^= entry.Key.GetHashCode();
            hashCode ^= _categoryMap.GetEntryOrFallback(entry.Key).Value.GetHashCode(entry.Value);
        }
        return hashCode;
    }

    public bool Satisfies
    (
        scoped ref readonly PackedMap<TKey, T> valueMap, 
        scoped ref readonly PackedMap<TKey, TCondition> conditionMap
    )
    {
        if (!_categoryMap.Fallback.Satisfies(in valueMap.Fallback, in conditionMap.Fallback)) {return false;}
        foreach (var valueEntry in valueMap.AsSpan)
        {
            var condition = conditionMap.GetEntryOrFallback(valueEntry.Key).Value;
            if (!_categoryMap.GetEntryOrFallback(valueEntry.Key).Value.Satisfies(in valueEntry.Value, in condition)) {return false;}
        }

        return true;
    }

    public bool IsSufficient
    (
        scoped ref readonly PackedMap<TKey, TCondition> sufficientMap, 
        scoped ref readonly PackedMap<TKey, TCondition> necessaryMap
    )
    {
        //--- Check fallback conditions ---
        if (!_categoryMap.Fallback.IsSufficient(in sufficientMap.Fallback, in necessaryMap.Fallback)) {return false;}
        //---|

        //--- Check dictionary members ---
        foreach (var entry in sufficientMap.AsSpan)
        {
            var necessaryCondition = necessaryMap.GetEntryOrFallback(entry.Key).Value;
            if (!_categoryMap.Fallback.IsSufficient(in entry.Value, in necessaryCondition)) {return false;}
        }
        //---|

        return true;
    }
}
