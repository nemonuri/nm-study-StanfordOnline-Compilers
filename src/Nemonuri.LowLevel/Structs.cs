
namespace Nemonuri.LowLevel;

public readonly struct MemoryAndSelector<T, TView>(Memory<T> memory, RefSelectorHandle<T, TView> selector)
{
    public readonly Memory<T> Memory = memory;
    public readonly RefSelectorHandle<T, TView> Selector = selector;
}

public readonly ref struct SpanAndSelector<T, TView>(Span<T> span, RefSelectorHandle<T, TView> selector)
{
    public readonly Span<T> Span = span;
    public readonly RefSelectorHandle<T, TView> Selector = selector;
}
