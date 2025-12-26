
namespace Nemonuri.LowLevel;

[StructLayout(LayoutKind.Sequential)]
public readonly struct LowLevelChoice<T1>
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
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct LowLevelChoice<T1, T2>
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
}
