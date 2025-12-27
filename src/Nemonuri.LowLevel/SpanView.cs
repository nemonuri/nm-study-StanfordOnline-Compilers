
namespace Nemonuri.LowLevel;

public readonly ref partial struct SpanView<T, TView> : ISpanView<TView>
{
    private readonly Span<T> _span;
    private readonly RefSelectorHandle<T, TView> _selectorHandle;

    public SpanView(Span<T> span, RefSelectorHandle<T, TView> selectorHandle)
    {
        _span = span;
        _selectorHandle = selectorHandle;
    }

    public int Length => _span.Length;

    public ref TView this[int index]
    {
        get
        {
            Guard.IsInRange(index, 0, Length);

            return ref _selectorHandle.Invoke(ref _span[index]);
        }
    }
}
