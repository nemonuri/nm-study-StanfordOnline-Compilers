
namespace Nemonuri.LowLevel.Extensions;

public static class MemoryExtensions
{
    extension<T>(in Memory<T> self)
    {
        public SpanView<T, TView> ToView<TView>(RefSelectorHandle<T, TView> selectorHandle)
        {
            return new(self.Span, selectorHandle);
        }

        public unsafe void 
        ToSpanViewOwner<TView>
        (
            RefSelectorHandle<T, TView> selectorHandle,
            out SpanViewHandle<MemoryAndSelector<T, TView>, T, TView> destHandle
        )
        {
            static void SpanViewProvider(in MemoryAndSelector<T, TView> owner, out SpanView<T, TView> spanView)
            {
                spanView = owner.Memory.ToView(owner.Selector);
            }

            SpanViewProviderHandle<MemoryAndSelector<T, TView>, T, TView> providerHandle = new(spanViewProvider: &SpanViewProvider);
            destHandle = new (owner: new(memory: self, selector: selectorHandle), spanViewProviderHandle: providerHandle);
        }
    }
}
