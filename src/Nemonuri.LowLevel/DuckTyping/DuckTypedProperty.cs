
namespace Nemonuri.LowLevel.DuckTyping;

public struct DuckTypedProperty<TReceiver, TResult> : IDuckTypedProperty<TReceiver, TResult>, IPropertyInvokable<TResult>
{
    private TReceiver _receiver;
    private readonly FunctionHandle<TReceiver, TResult> _propertyHandle;

    public DuckTypedProperty(TReceiver receiver, FunctionHandle<TReceiver, TResult> propertyHandle)
    {
        _receiver = receiver;
        _propertyHandle = propertyHandle;
    }

    [UnscopedRef]
    public ref readonly TReceiver Receiver => ref _receiver;

    public FunctionHandle<TReceiver, TResult> PropertyHandle => _propertyHandle;

    [UnscopedRef]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TResult? InvokeProperty() =>
        ref _propertyHandle.InvokeFunction(ref _receiver!);
}
