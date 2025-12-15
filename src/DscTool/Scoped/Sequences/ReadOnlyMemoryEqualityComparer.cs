
namespace DscTool.Scoped.Sequences;

public readonly struct ReadOnlyMemoryEqualityComparer<T, TEqualityComparer> :
    IEqualityComparer<ReadOnlyMemory<T>>
    where TEqualityComparer : IEqualityComparer<T>
{
    private readonly TEqualityComparer _equalityComparer;

    public ReadOnlyMemoryEqualityComparer(TEqualityComparer equalityComparer)
    {
        _equalityComparer = equalityComparer;
    }

    public bool Equals(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y)
    {
        scoped var xs = x.Span;
        scoped var ys = y.Span;

        int length = xs.Length;
        if (length != ys.Length) {return false;}

        for (int i = 0; i < length; i++)
        {
            if (!_equalityComparer.Equals(xs[i], ys[i])) {return false;}
        }
        return true;
    }

    public int GetHashCode(ReadOnlyMemory<T> obj)
    {
        HashCode hc = default;
        foreach (var item in obj.Span)
        {
            hc.Add(_equalityComparer.GetHashCode(item));
        }
        return hc.ToHashCode();
    }
}
