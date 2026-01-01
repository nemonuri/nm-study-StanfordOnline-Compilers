
namespace Nemonuri.LowLevel;

[StructLayout(LayoutKind.Sequential)]
public record struct SequentialLayoutValueTuple<T1, T2>
{
    public T1? Item1;
    public T2? Item2;

    public SequentialLayoutValueTuple(T1? item1, T2? item2)
    {
        Item1 = item1;
        Item2 = item2;
    }
}

[StructLayout(LayoutKind.Sequential)]
public record struct SequentialLayoutValueTuple<T1, T2, T3>
{
    public T1? Item1;
    public T2? Item2;
    public T3? Item3;

    public SequentialLayoutValueTuple(T1? item1, T2? item2, T3? item3)
    {
        Item1 = item1;
        Item2 = item2;
        Item3 = item3;
    }
}