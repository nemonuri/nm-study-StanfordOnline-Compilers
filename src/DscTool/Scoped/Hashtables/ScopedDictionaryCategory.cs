using DscTool.Infrastructure;

namespace DscTool.Scoped.Hashtables;

public readonly struct ScopedDictionaryCategory<T, TCondition, TCategory, TKey> :
    IScopedCategory<Memory<KeyValuePair<TKey, T>>, ReadOnlyDictionaryFallbackPair<TKey, TCondition>>
    where TCategory : IScopedCategory<T, TCondition>
    where TKey : IEquatable<TKey>
{
    private readonly ReadOnlyDictionaryFallbackPair<TKey, TCategory> _categoryTable;

    public ScopedDictionaryCategory(ReadOnlyDictionaryFallbackPair<TKey, TCategory> categoryTable)
    {
        _categoryTable = categoryTable;
    }

    public bool Equals(Memory<KeyValuePair<TKey, T>> xMemory, Memory<KeyValuePair<TKey, T>> yMemory)
    {
        scoped var xs = xMemory.Span;
        scoped var ys = yMemory.Span;

        int length = xs.Length;
        if (length != ys.Length) {return false;}

        for (int i = 0; i < length; i++)
        {
            ref var x = ref xs[i];
            ref var y = ref ys[i];
            var key = x.Key;

            if (!key.Equals(y.Key)) {return false;}
            if (!_categoryTable.GetValueOrFallback(key).Equals(x.Value, y.Value)) {return false;}
        }

        return false;
    }

    public int GetHashCode(Memory<KeyValuePair<TKey, T>> xMemory)
    {
        scoped var xs = xMemory.Span;

        HashCode hc = default;
        for (int i = 0; i < xs.Length; i++)
        {
            ref var x = ref xs[i];
            var key = x.Key;

            hc.Add(key);
            hc.Add(_categoryTable.GetValueOrFallback(key).GetHashCode(x.Value));
        }
        return hc.ToHashCode();
    }

    public bool Satisfies(scoped ref readonly Memory<KeyValuePair<TKey, T>> value, scoped ref readonly ReadOnlyDictionaryFallbackPair<TKey, TCondition> condition)
    {
        var valueSpan = value.Span;

        for (int i = 0; i < valueSpan.Length; i++)
        {
            ref var v = ref valueSpan[i];
            var key = v.Key;
            var value0 = v.Value;
            var c = condition.GetValueOrFallback(key);

            if (!_categoryTable.GetValueOrFallback(key).Satisfies(in value0, in c)) {return false;};
        }

        return true;
    }

    public bool IsSufficient
    (
        scoped ref readonly ReadOnlyDictionaryFallbackPair<TKey, TCondition> sufficient, 
        scoped ref readonly ReadOnlyDictionaryFallbackPair<TKey, TCondition> necessary
    )
    {
        ref readonly var fallbackCategory = ref _categoryTable.Fallback;

        //--- Check fallback conditions ---
        if (!fallbackCategory.IsSufficient(in sufficient.Fallback, in necessary.Fallback)) {return false;}
        //---|

        //--- Check dictionary members ---
        foreach (var entry in sufficient.Dictionary)
        {
            var condl = entry.Value;
            var condr = necessary.GetValueOrFallback(entry.Key);
            if (!_categoryTable.Fallback.IsSufficient(in condl, in condr)) {return false;}
        }
        //---|

        return true;
    }
}


