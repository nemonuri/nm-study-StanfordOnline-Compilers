
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

public struct DuckTypedMethod<TReceiver, TArgument, TResult> : IDuckTypedMethod<TReceiver, TArgument, TResult>
{
    private TReceiver _receiver;
    private readonly MethodHandle<TReceiver, TArgument, TResult> _methodHandle;

    public DuckTypedMethod(TReceiver receiver, MethodHandle<TReceiver, TArgument, TResult> methodHandle)
    {
        _receiver = receiver;
        _methodHandle = methodHandle;
    }

    [UnscopedRef]
    public ref TReceiver Receiver => ref _receiver;

    public MethodHandle<TReceiver, TArgument, TResult> MethodHandle => _methodHandle;
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
}
