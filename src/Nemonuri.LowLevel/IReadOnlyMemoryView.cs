namespace Nemonuri.LowLevel;

public interface IReadOnlyMemoryView<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    int Length {get;}

    [UnscopedRef] ref readonly T this[int index] {get;}
}
