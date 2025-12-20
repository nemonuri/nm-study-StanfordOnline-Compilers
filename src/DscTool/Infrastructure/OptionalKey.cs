
namespace DscTool.Infrastructure;

public readonly struct OptionalKey<TKey> : IEquatable<OptionalKey<TKey>>
    where TKey : IEquatable<TKey>
{
    public readonly struct Some(TKey value) 
    { 
        public readonly TKey Value = value;
    }

    private readonly int _index;
    private const int IndexNone = 0;
    private const int IndexSome = 1;
    private readonly Some _some;

    private OptionalKey(int index, Some some)
    {
        _index = index;
        _some = some;
    }

    public bool IsInvalid => !((IndexNone <= _index) && (_index <= IndexSome));
    public bool IsNone => _index == IndexNone;
    public bool IsSome => _index == IndexSome;

    public TKey GetSome()
    {
        Guard.IsTrue(IsSome);
        return _some.Value;
    }

    public static implicit operator OptionalKey<TKey>(Nil nil) => new(IndexNone, default);
    public static implicit operator OptionalKey<TKey>(Some some) => new(IndexSome, some);

    public bool Equals(OptionalKey<TKey> other)
    {
        return (_index == other._index) &&
                (!IsSome || GetSome().Equals(other.GetSome()));
    }

    public override bool Equals(object obj) => obj switch
    {
        OptionalKey<TKey> okey => Equals(okey),
        Nil nil => Equals((OptionalKey<TKey>)nil),
        Some some => Equals((OptionalKey<TKey>)some),
        TKey key => Equals(OptionalKeyTagger.Some(key)),
        _ => false
    };
    
    public override int GetHashCode()
    {
        HashCode hc = default;
        hc.Add(_index);
        if (IsSome)
        {
            hc.Add(GetSome());
        }
        return hc.ToHashCode();
    }

    public static bool operator ==(OptionalKey<TKey> left, OptionalKey<TKey> right) => left.Equals(right);
    public static bool operator !=(OptionalKey<TKey> left, OptionalKey<TKey> right) => !(left == right);
}

public static class OptionalKeyTagger
{
    public readonly static Nil None = default;

    public static OptionalKey<TKey>.Some Some<TKey>(TKey key) where TKey : IEquatable<TKey>
        => new(key);
}
