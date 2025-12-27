
namespace Nemonuri.LowLevel;

public readonly struct SpanViewHandle<TOwner, T, TView> : ISpanViewOwner<T, TView>
{
    private readonly TOwner _owner;
    
    private readonly SpanViewProviderHandle<TOwner, T, TView> _spanViewProviderHandle;

    public SpanViewHandle(TOwner owner, SpanViewProviderHandle<TOwner, T, TView> spanViewProviderHandle)
    {
        _owner = owner;
        _spanViewProviderHandle = spanViewProviderHandle;
    }

    public void GetSpanView(scoped ref SpanView<T, TView> spanView) => _spanViewProviderHandle.Invoke(in _owner, ref spanView);
}
