
namespace DscTool.Scoped.Sequences;

public readonly struct ReadOnlyMemoryEqualityComparer<T, TEqualityComparer> :
    IEqualityComparer<ReadOnlyMemory<T>>
    where TEqualityComparer : IEqualityComparer<T>
{
    private readonly ReadOnlyMemory<TEqualityComparer> _equalityComparers;

    public ReadOnlyMemoryEqualityComparer(ReadOnlyMemory<TEqualityComparer> equalityComparers)
    {
        _equalityComparers = equalityComparers;
    }

    public bool Equals(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y)
    {
        scoped var ecs = _equalityComparers.Span;
        int length = ecs.Length;

        scoped var xs = x.Span;
        scoped var ys = y.Span;
        
        if (length != xs.Length) {return false;}
        if (length != ys.Length) {return false;}

        for (int i = 0; i < length; i++)
        {
            if (!ecs[i].Equals(xs[i], ys[i])) {return false;}
        }
        return true;
    }

    public int GetHashCode(ReadOnlyMemory<T> obj)
    {
        scoped var ecs = _equalityComparers.Span;
        int length = ecs.Length;

        scoped var objs = obj.Span;

        if (length != objs.Length) {return 0;}

        HashCode hc = default;
        for (int i = 0; i < length; i++)
        {
            hc.Add(ecs[i].GetHashCode(objs[i]));
        }
        return hc.ToHashCode();
    }
}
