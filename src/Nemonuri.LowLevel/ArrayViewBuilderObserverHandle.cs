namespace Nemonuri.LowLevel;

[StructLayout(LayoutKind.Sequential)]
public readonly struct ArrayViewBuilderObserverHandle<TReceiver, T>
{
    private readonly BuilderObserverHandle<TReceiver, T, ArrayView<T>, ArrayViewObservableBuilder<T, TReceiver>> _handle;
}

public static class ArrayViewBuilderObserverHandle
{
    extension<TReceiver, T>(ref BuilderObserverHandle<TReceiver, T, ArrayView<T>, ArrayViewObservableBuilder<T, TReceiver>> wrapping)
    {
        public ref ArrayViewBuilderObserverHandle<TReceiver, T> Wrap() => 
            ref Unsafe.As<
                BuilderObserverHandle<TReceiver, T, ArrayView<T>, ArrayViewObservableBuilder<T, TReceiver>>, 
                ArrayViewBuilderObserverHandle<TReceiver, T>>
                (ref wrapping);
    }

    extension<TReceiver, T>(ref ArrayViewBuilderObserverHandle<TReceiver, T> wrapped)
    {
        public ref BuilderObserverHandle<TReceiver, T, ArrayView<T>, ArrayViewObservableBuilder<T, TReceiver>> Unwrap() =>
            ref Unsafe.As<
                ArrayViewBuilderObserverHandle<TReceiver, T>,
                BuilderObserverHandle<TReceiver, T, ArrayView<T>, ArrayViewObservableBuilder<T, TReceiver>>>
                (ref wrapped);
    }
}