
namespace Nemonuri.LowLevel.DuckTyping;

public struct MemoryViewDuckTypedProvider<T, TMemoryView, TReceiver, TArgument> :
    IMemoryViewProvider<T, TMemoryView>
    where TMemoryView : IMemoryView<T>
{
    private TReceiver _receiver;
    private TArgument? _argument;
    public readonly MethodHandle<TReceiver, TArgument, TMemoryView> MethodHandle;

    public MemoryViewDuckTypedProvider(TReceiver receiver, TArgument? argument, MethodHandle<TReceiver, TArgument, TMemoryView> methodHandle)
    {
        _receiver = receiver;
        _argument = argument;
        MethodHandle = methodHandle;
    }

    [UnscopedRef] public ref readonly TReceiver Receiver => ref _receiver;

    [UnscopedRef] public ref readonly TArgument? Argument => ref _argument;

    [UnscopedRef]
    public ref readonly TMemoryView? InvokeProvider() => ref MethodHandle.InvokeMethod(ref _receiver, ref _argument);
}
