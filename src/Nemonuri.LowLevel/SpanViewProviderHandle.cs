
namespace Nemonuri.LowLevel;

public unsafe readonly struct SpanViewProviderHandle<TSource, T, TView>
#if NET9_0_OR_GREATER
    where TSource : allows ref struct
#endif
{
    private readonly delegate*<in TSource, out SpanView<T, TView>, void> _spanViewProvider;

    public SpanViewProviderHandle(delegate*<in TSource, out SpanView<T, TView>, void> spanViewProvider)
    {
        Guard.IsTrue(spanViewProvider is not null);
        _spanViewProvider = spanViewProvider;
    }

#pragma warning disable CS9088
    public void Invoke(scoped in TSource owner, scoped ref SpanView<T, TView> spanView) => 
        _spanViewProvider(in owner, out spanView);
#pragma warning restore CS9088
}
