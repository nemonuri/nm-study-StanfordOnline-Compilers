namespace Nemonuri.LowLevel;

public interface ILowLevelReadOnlyList<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    int Length {get;}

    T this[int index] {get;}
}
