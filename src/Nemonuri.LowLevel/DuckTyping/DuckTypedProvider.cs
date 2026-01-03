
namespace Nemonuri.LowLevel.DuckTyping;

public readonly struct DuckTypedProvider<TReceiver, TItem> : IDuckTypedProvider<TReceiver, TItem>, IProviderInvokable<TItem>
{
    private readonly TReceiver _receiver;
    private readonly ProviderHandle<TReceiver, TItem> _providerHandle;

    public DuckTypedProvider(TReceiver receiver, ProviderHandle<TReceiver, TItem> providerHandle)
    {
        _receiver = receiver;
        _providerHandle = providerHandle;
    }

    public ProviderHandle<TReceiver, TItem> ProviderHandle => _providerHandle;

    [UnscopedRef]
    public ref readonly TReceiver Receiver => ref _receiver;

    [UnscopedRef]
    public ref readonly TItem? InvokeProvider() => ref _providerHandle.InvokeProvider(in _receiver);
}