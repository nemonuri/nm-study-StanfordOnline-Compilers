namespace Nemonuri.LowLevel;

[StructLayout(LayoutKind.Sequential)]
public readonly struct MemoryViewReceiverBuilderObserverHandle<TReceiver, T>
{
    private readonly BuilderObserverHandle<TReceiver, T, MemoryViewReceiver<TReceiver, T>, MemoryViewReceiver<TReceiver, T>.ObservableBuilder> _handle;
}

public static class MemoryViewReceiverBuilderObserverHandle
{
    extension<TReceiver, T>(ref BuilderObserverHandle<TReceiver, T, MemoryViewReceiver<TReceiver, T>, MemoryViewReceiver<TReceiver, T>.ObservableBuilder> wrapping)
    {
        public ref MemoryViewReceiverBuilderObserverHandle<TReceiver, T> Wrap() => 
            ref Unsafe.As<
                BuilderObserverHandle<TReceiver, T, MemoryViewReceiver<TReceiver, T>, MemoryViewReceiver<TReceiver, T>.ObservableBuilder>, 
                MemoryViewReceiverBuilderObserverHandle<TReceiver, T>>
                (ref wrapping);
    }

    extension<TReceiver, T>(ref MemoryViewReceiverBuilderObserverHandle<TReceiver, T> wrapped)
    {
        public ref BuilderObserverHandle<TReceiver, T, MemoryViewReceiver<TReceiver, T>, MemoryViewReceiver<TReceiver, T>.ObservableBuilder> Unwrap() =>
            ref Unsafe.As<
                MemoryViewReceiverBuilderObserverHandle<TReceiver, T>,
                BuilderObserverHandle<TReceiver, T, MemoryViewReceiver<TReceiver, T>, MemoryViewReceiver<TReceiver, T>.ObservableBuilder>>
                (ref wrapped);
    }
}