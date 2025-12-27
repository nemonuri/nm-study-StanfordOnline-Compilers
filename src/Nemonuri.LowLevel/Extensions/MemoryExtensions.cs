
namespace Nemonuri.LowLevel.Extensions;

public static class MemoryExtensions
{
    extension<T>(in Memory<T> self)
    {
        public SpanView<T, TView> ToView<TView>(RefSelectorHandle<T, TView> selectorHandle)
        {
            return new(self.Span, selectorHandle);
        }
    }
}
