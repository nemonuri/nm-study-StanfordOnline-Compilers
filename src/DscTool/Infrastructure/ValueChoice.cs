
namespace DscTool.Infrastructure;

public readonly record struct TagChoice1<T>(T Value);
public readonly record struct TagChoice2<T>(T Value);

public readonly struct ValueChoice<T1, T2> : IEquatable<ValueChoice<T1, T2>>
{
    private readonly int _index;

    private const int IndexNone = 0;
    private const int IndexChoice1 = 1;
    private readonly TagChoice1<T1> _choice1;

    private const int IndexChoice2 = 2;
    private readonly TagChoice2<T2> _choice2;

    public ValueChoice(int index, TagChoice1<T1> choice1, TagChoice2<T2> choice2)
    {
        _index = index;
        _choice1 = choice1; _choice2 = choice2;
    }

    public bool IsNone => _index == IndexNone;
    public bool IsChoice1 => _index == IndexChoice1;
    public bool IsChoice2 => _index == IndexChoice2;

    public T1 GetChoice1()
    {
        Guard.IsTrue(IsChoice1);
        return _choice1.Value;
    }

    public T2 GetChoice2()
    {
        Guard.IsTrue(IsChoice2);
        return _choice2.Value;
    }

    public static implicit operator ValueChoice<T1, T2>(Nil nil) => new(IndexNone, default, default);
    public static implicit operator ValueChoice<T1, T2>(TagChoice1<T1> choice1) => new(IndexChoice1, choice1, default);
    public static implicit operator ValueChoice<T1, T2>(TagChoice2<T2> choice2) => new(IndexChoice2, default, choice2);

    public bool Equals(ValueChoice<T1, T2> other)
    {
        if (_index != other._index) {return false;}
        if (IsChoice1) {return GetChoice1()?.Equals(other.GetChoice1()) ?? false;}
        if (IsChoice2) {return GetChoice2()?.Equals(other.GetChoice2()) ?? false;}
        return true;
    }

    public override bool Equals(object obj) => obj is ValueChoice<T1, T2> vc && Equals(vc);
    
    public override int GetHashCode()
    {
        HashCode hc = default;
        hc.Add(_index);
        if (IsChoice1)
        {
            hc.Add(GetChoice1());
        }
        else if (IsChoice2)
        {
            hc.Add(GetChoice2());
        }
        return hc.ToHashCode();
    }
}

public static class ValueChoiceTagger
{
    public static readonly Nil None = default;
    public static TagChoice1<T> Choice1<T>(T value) => new(value);
    public static TagChoice2<T> Choice2<T>(T value) => new(value);
}
