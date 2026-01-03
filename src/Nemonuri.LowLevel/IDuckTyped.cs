
using System.Diagnostics;
using Nemonuri.LowLevel.Primitives;

namespace Nemonuri.LowLevel;

public interface IDuckTypeReceiver<TReceiver>
{
    [UnscopedRef] ref readonly TReceiver Receiver {get;}
}

public interface IDuckTypedMethod<TReceiver, TArgument, TResult> : IDuckTypeReceiver<TReceiver>
{
    MethodHandle<TReceiver, TArgument, TResult> MethodHandle {get;}
}

public struct DuckTypedMethod<TReceiver, TSource, TResult> : IDuckTypedMethod<TReceiver, TSource, TResult>
{
    private TReceiver _receiver;
    private readonly MethodHandle<TReceiver, TSource, TResult> _methodHandle;

    public DuckTypedMethod(TReceiver receiver, MethodHandle<TReceiver, TSource, TResult> methodHandle)
    {
        _receiver = receiver;
        _methodHandle = methodHandle;
    }

    [UnscopedRef]
    public ref readonly TReceiver Receiver => ref _receiver;

    public readonly MethodHandle<TReceiver, TSource, TResult> MethodHandle => _methodHandle;

    [UnscopedRef]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TResult? InvokeMethod(ref TSource? source) =>
        ref _methodHandle.InvokeMethod(ref _receiver, ref source);
}

public interface IDuckTypedProperty<TReceiver, TResult> : IDuckTypeReceiver<TReceiver>
{
    FunctionHandle<TReceiver, TResult> PropertyHandle {get;}
}

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

public interface IDuckTypedProvider<TReceiver, TItem> : IDuckTypeReceiver<TReceiver>
{
    ProviderHandle<TReceiver, TItem> ProviderHandle {get;}
}

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