
using Nemonuri.LowLevel.Extensions;

namespace Nemonuri.LowLevel;

public readonly struct MemoryView<T, TView> : ISpanView<TView>, ISpanViewOwner<T, TView>
{
    private readonly Memory<T> _memory;
    private readonly RefSelectorHandle<T, TView> _selectorHandle;

    public int Length => _memory.Length;

    public ref TView this[int index] => ref _selectorHandle.Invoke(ref _memory.Span[index]);

    public void GetSpanView(scoped ref SpanView<T, TView> spanView)
    {
        _memory.ToView(_selectorHandle, ref spanView);
    }
}
