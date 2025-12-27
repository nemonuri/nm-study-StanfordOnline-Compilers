
namespace Nemonuri.LowLevel;

[StructLayout(LayoutKind.Sequential)]
public readonly struct LowLevelChoice<T1> : IEquatable<LowLevelChoice<T1>>
{
    //--- Fields ---
    public readonly int Index;
    public readonly Choice1Tagged<T1> Choice1;
    //---|

    //--- Constructors ---
    private LowLevelChoice(int index, Choice1Tagged<T1> choice1)
    {
        Index = index;
        Choice1 = choice1;
    }

    public static implicit operator LowLevelChoice<T1>(Nil nil) => new(0, default);
    public static implicit operator LowLevelChoice<T1>(Choice1Tagged<T1> choice1) => new(1, choice1);
    //---|

    //--- Validators ---
    public bool IsIndexInValidRange => (0 <= Index) && (Index <= 1);
    public bool IsNone => Index == 0;
    public bool IsChoice1 => Index == 1;
    //---|

    //--- Equality ---
    public bool Equals(LowLevelChoice<T1> other)
    {
        if (Index != other.Index) {return false;}
        if (IsChoice1)
        {
            return EqualityComparer<T1>.Default.Equals(Choice1.Value, other.Choice1.Value);
        }
        return true;
    }

    public override bool Equals(object? obj) => obj is LowLevelChoice<T1> v && Equals(v);
    
    public override int GetHashCode()
    {
        if (IsChoice1 && Choice1.Value is { } cv1)
        {
            return HashCode.Combine(Index, EqualityComparer<T1>.Default.GetHashCode(cv1));
        }
        return 0;
    }

    public static bool operator ==(LowLevelChoice<T1> left, LowLevelChoice<T1> right) => left.Equals(right);
    public static bool operator !=(LowLevelChoice<T1> left, LowLevelChoice<T1> right) => !(left == right);
    //---|
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct LowLevelChoice<T1, T2> : IEquatable<LowLevelChoice<T1, T2>>
{
    //--- Fields ---
    public readonly int Index;
    public readonly Choice1Tagged<T1> Choice1;
    public readonly Choice2Tagged<T2> Choice2;
    //---|

    //--- Constructors ---
    private LowLevelChoice(int index, Choice1Tagged<T1> choice1, Choice2Tagged<T2> choice2)
    {
        Index = index;
        Choice1 = choice1;
        Choice2 = choice2;
    }

    public static implicit operator LowLevelChoice<T1, T2>(Nil nil) => new(0, default, default);
    public static implicit operator LowLevelChoice<T1, T2>(Choice1Tagged<T1> choice1) => new(1, choice1, default);
    public static implicit operator LowLevelChoice<T1, T2>(Choice2Tagged<T2> choice2) => new(2, default, choice2);
    //---|

    //--- Validators ---
    public bool IsIndexInValidRange => (0 <= Index) && (Index <= 2);
    public bool IsNone => Index == 0;
    public bool IsChoice1 => Index == 1;
    public bool IsChoice2 => Index == 2;
    //---|

    //--- Equality ---
    public bool Equals(LowLevelChoice<T1, T2> other)
    {
        if (Index != other.Index) {return false;}
        if (IsChoice1)
        {
            return EqualityComparer<T1>.Default.Equals(Choice1.Value, other.Choice1.Value);
        }
        else if (IsChoice2)
        {
            return EqualityComparer<T2>.Default.Equals(Choice2.Value, other.Choice2.Value);
        }
        return true;
    }

    public override bool Equals(object? obj) => obj is LowLevelChoice<T1, T2> v && Equals(v);
    
    public override int GetHashCode()
    {
        if (IsChoice1 && Choice1.Value is { } cv1)
        {
            return HashCode.Combine(Index, EqualityComparer<T1>.Default.GetHashCode(cv1));
        }
        else if (IsChoice2 && Choice2.Value is { } cv2)
        {
            return HashCode.Combine(Index, EqualityComparer<T2>.Default.GetHashCode(cv2));
        }
        return 0;
    }

    public static bool operator ==(LowLevelChoice<T1, T2> left, LowLevelChoice<T1, T2> right) => left.Equals(right);
    public static bool operator !=(LowLevelChoice<T1, T2> left, LowLevelChoice<T1, T2> right) => !(left == right);
    //---|
}
