
namespace Nemonuri.LowLevel;

public static class LowLevelChoiceTagger
{
    public static readonly Nil None = default;
    public static Choice1Tagged<T> Choice1<T>(in T value) => new(value);
    public static Choice2Tagged<T> Choice2<T>(in T value) => new(value);
}
