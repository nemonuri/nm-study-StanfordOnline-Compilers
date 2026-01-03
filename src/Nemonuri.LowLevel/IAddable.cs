namespace Nemonuri.LowLevel;

public interface IAddable<TSource>
{
    void Add(in TSource source);
}
