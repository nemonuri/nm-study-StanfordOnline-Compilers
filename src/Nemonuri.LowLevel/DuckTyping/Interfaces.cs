
namespace Nemonuri.LowLevel.DuckTyping;

public interface IDuckTypeReceiver<TReceiver>
{
    [UnscopedRef] ref readonly TReceiver Receiver {get;}
}

public interface IDuckTypedMethod<TReceiver, TArgument, TResult> : IDuckTypeReceiver<TReceiver>
{
    MethodHandle<TReceiver, TArgument, TResult> MethodHandle {get;}
}

public interface IDuckTypedProperty<TReceiver, TResult> : IDuckTypeReceiver<TReceiver>
{
    FunctionHandle<TReceiver, TResult> PropertyHandle {get;}
}

public interface IDuckTypedProvider<TReceiver, TItem> : IDuckTypeReceiver<TReceiver>
{
    ProviderHandle<TReceiver, TItem> ProviderHandle {get;}
}
