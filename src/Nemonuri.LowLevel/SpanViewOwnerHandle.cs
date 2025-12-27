
namespace Nemonuri.LowLevel;

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

    public void Invoke(out SpanView<T, TView> spanView) => _spanViewProviderHandle.Invoke(in _owner, out spanView);
}
