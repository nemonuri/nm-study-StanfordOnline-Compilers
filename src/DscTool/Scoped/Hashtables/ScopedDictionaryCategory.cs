using DscTool.Scoped.Sequences;
using DscTool.Infrastructure;

namespace DscTool.Scoped.Hashtables;

public readonly struct ScopedDictionaryCategory<T, TCondition, TCategory, TDictionary> :
    IScopedCategory<Memory<T>, Memory<TCondition>>
    where TCategory : IScopedCategory<T, TCondition>
    where TDictionary : IValueEntryDictionary<T, TCondition, TCategory>
{
    private readonly TDictionary _dictionary;

    public ScopedDictionaryCategory(TDictionary dictionary)
    {
        _dictionary = dictionary;
    }

    public bool Equals(Memory<T> xMemory, Memory<T> yMemory)
    {
        scoped var xs = xMemory.Span;
        scoped var ys = yMemory.Span;

        int length = xs.Length;
        if (length != ys.Length) {return false;}

        for (int i = 0; i < length; i++)
        {
            T x = xs[i];
            if (!_dictionary.TryGetValue(x, out var ve)) {return false;}
            if (!ve.Category.Equals(x, ys[i])) {return false;}
        }

        return true;
    }

    public int GetHashCode(Memory<T> xMemory)
    {
        scoped var xs = xMemory.Span;
        int length = xs.Length;

        HashCode hc = default;
        for (int i = 0; i < length; i++)
        {
            T x = xs[i];
            if (!_dictionary.TryGetValue(x, out var ve)) {return 0;}

            hc.Add(ve.Category.GetHashCode(x));
        }
        return hc.ToHashCode();
    }

    public bool Satisfies(scoped ref readonly Memory<T> valueMemory, scoped ref readonly Memory<TCondition> conditionMemory)
    {
        scoped var vs = valueMemory.Span;
        scoped var cs = conditionMemory.Span;
        int length = vs.Length;
        if (length != cs.Length) {return false;}

        for (int i = 0; i < length; i++)
        {
            T v = vs[i];
            if (!_dictionary.TryGetValue(v, out var ve)) {return false;}
            if (!ve.Category.Satisfies(in v, in cs[i])) {return false;}
        }

        return true;
    }

    public bool IsSufficient(scoped ref readonly Memory<TCondition> sufficientMemory, scoped ref readonly Memory<TCondition> necessaryMemory)
    {
        scoped var ss = sufficientMemory.Span;
        scoped var ns = necessaryMemory.Span;
        int length = ss.Length;
        if (length != ns.Length) {return false;}

        for (int i = 0; i < length; i++)
        {
            TCondition s = ss[i];
            if (!_dictionary.TryGetValue(v, out var ve)) {return false;}
            if (!ve.Category.Satisfies(in v, in cs[i])) {return false;}
        }

    }
}


