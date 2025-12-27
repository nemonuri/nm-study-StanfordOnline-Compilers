
namespace Nemonuri.LowLevel;

public static class LowLevelChoiceTagger
{
    public static readonly Nil None = default;
    public static ref readonly Choice1Tagged<T> Choice1<T>(in T value) => ref UnsafeReadOnly.As<T, Choice1Tagged<T>>(in value);
    public static ref readonly Choice2Tagged<T> Choice2<T>(in T value) => ref UnsafeReadOnly.As<T, Choice2Tagged<T>>(in value);
}
