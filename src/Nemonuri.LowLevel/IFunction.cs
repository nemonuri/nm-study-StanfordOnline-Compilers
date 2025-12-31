namespace Nemonuri.LowLevel;

public interface IFunction<T, TOut>
{
    TOut? InvokeFunction(ref T arg);
}

public unsafe readonly struct FunctionHandle<TReceiver, T, TOut>
{
    private readonly delegate*<ref TReceiver, ref T, TOut?> _pInvokeFunction;

    public FunctionHandle(delegate*<ref TReceiver, ref T, TOut?> pInvokeFunction)
    {
        LowLevelGuard.IsNotNull(pInvokeFunction);
        _pInvokeFunction = pInvokeFunction;
    }

    public TOut? InvokeFunction(ref TReceiver receiver, ref T arg) => _pInvokeFunction(ref receiver, ref arg);
}

public static class FunctionTheory
{
    extension<T, TOut, TFunction>(TFunction func)
        where TFunction : IFunction<T, TOut>
    {
        public FunctionHandle<TFunction, T, TOut> ToFunctionHandle()
        {
            static TOut? InvokeImpl(ref TFunction func0, ref T arg) => func0.InvokeFunction(ref arg);

            unsafe { return new(&InvokeImpl); }
        }
    }
}