
namespace Nemonuri.LowLevel.DuckTyping;

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
