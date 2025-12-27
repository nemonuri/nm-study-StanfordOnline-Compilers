namespace Nemonuri.LowLevel;

public interface IMemoryView<TView>
{
    int Length {get;}

    ref TView this[int index] {get;}
}
