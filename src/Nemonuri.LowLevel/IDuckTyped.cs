
using System.Diagnostics;
using Nemonuri.LowLevel.Primitives;

namespace Nemonuri.LowLevel;

public interface IDuckTypeReceiver<TReceiver>
{
    [UnscopedRef] ref TReceiver Receiver {get;}
}

public interface IDuckTypedMethod<TReceiver, TArgument, TResult> : IDuckTypeReceiver<TReceiver>
{
    MethodHandle<TReceiver, TArgument, TResult> MethodHandle {get;}
}

public static class DuckTypedMethodTheory
{
    extension<TReceiver, TArgument, TResult, TDuckTypedMethod>(ref TDuckTypedMethod method)
        where TDuckTypedMethod : struct, IDuckTypedMethod<TReceiver, TArgument, TResult>
    {
        public unsafe ref TResult? InvokeMethod(ref TArgument? argument) => ref method.MethodHandle.FunctionPointer(ref method.Receiver, ref argument);
    }
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
    public ref TReceiver Receiver => ref _receiver;

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

public struct DuckTypedProperty<TReceiver, TResult> : IDuckTypedProperty<TReceiver, TResult>
{
    private TReceiver _receiver;
    private readonly FunctionHandle<TReceiver, TResult> _propertyHandle;

    public DuckTypedProperty(TReceiver receiver, FunctionHandle<TReceiver, TResult> propertyHandle)
    {
        _receiver = receiver;
        _propertyHandle = propertyHandle;
    }

    [UnscopedRef]
    public ref TReceiver Receiver => ref _receiver;

    public FunctionHandle<TReceiver, TResult> PropertyHandle => _propertyHandle;

    [UnscopedRef]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TResult? InvokeProperty() =>
        ref _propertyHandle.InvokeFunction(ref _receiver!);
}
