namespace Nemonuri.LowLevel;

public interface IFunction<T, TOut>
{
    TOut? Invoke(ref T arg);
}

public unsafe readonly struct FunctionHandle<TReceiver, T, TOut>
{
    private readonly delegate*<ref TReceiver, ref T, TOut?> _pInvoke;

    public FunctionHandle(delegate*<ref TReceiver, ref T, TOut?> pInvoke)
    {
        LowLevelGuard.IsNotNull(pInvoke);
        _pInvoke = pInvoke;
    }

    public TOut? InvokeInvoke(ref TReceiver receiver, ref T arg) => _pInvoke(ref receiver, ref arg);
}