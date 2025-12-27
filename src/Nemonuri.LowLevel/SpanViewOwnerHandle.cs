
namespace Nemonuri.LowLevel;

public readonly struct SpanViewOwnerHandle<TOwner, T, TView>
{
    private readonly TOwner _owner;
    
    private readonly SpanViewProviderHandle<TOwner, T, TView> _spanViewProviderHandle;

    public SpanViewOwnerHandle(TOwner owner, SpanViewProviderHandle<TOwner, T, TView> spanViewProviderHandle)
    {
        _owner = owner;
        _spanViewProviderHandle = spanViewProviderHandle;
    }

    public void Invoke(scoped ref SpanView<T, TView> spanView) => _spanViewProviderHandle.Invoke(in _owner, ref spanView);
}
