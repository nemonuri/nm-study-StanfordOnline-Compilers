
namespace Nemonuri.LowLevel;

public unsafe readonly struct RefSelectorHandle<TSource, TResult>
{
    private readonly delegate*<ref TSource, ref TResult> _selector;

    public RefSelectorHandle(delegate*<ref TSource, ref TResult> selector)
    {
        Guard.IsTrue(selector is not null);
        _selector = selector;
    }

    public ref TResult Invoke(ref TSource source) => ref _selector(ref source);
}

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
    public void Invoke(scoped in TSource owner, out SpanView<T, TView> spanView) => 
        _spanViewProvider(in owner, out spanView);
#pragma warning restore CS9088
}

public readonly ref struct SpanViewOwnerHandle<TOwner, T, TView>
#if NET9_0_OR_GREATER
    where TOwner : allows ref struct
#endif
{
    private readonly TOwner _owner;
    
    private readonly SpanViewProviderHandle<TOwner, T, TView> _spanViewProviderHandle;

    public SpanViewOwnerHandle(TOwner owner, SpanViewProviderHandle<TOwner, T, TView> spanViewProviderHandle)
    {
        _owner = owner;
        _spanViewProviderHandle = spanViewProviderHandle;
    }

    public void Invoke(out SpanView<T, TView> spanView) => _spanViewProviderHandle.Invoke(_owner, out spanView);
}
