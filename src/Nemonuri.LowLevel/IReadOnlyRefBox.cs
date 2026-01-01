namespace Nemonuri.LowLevel;

public interface IReadOnlyRefBox<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    [UnscopedRef] ref readonly T? ReadOnlyRefValue {get;}
}