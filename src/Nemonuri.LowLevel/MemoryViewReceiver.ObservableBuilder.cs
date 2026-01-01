
namespace Nemonuri.LowLevel;

public partial struct MemoryViewReceiver<TReceiver, T>
{
    public struct ObservableBuilder : IBuilder<T, MemoryViewReceiver<TReceiver, T>>
    {
        private ArrayViewObservableBuilder<T, TReceiver> _builder;
        private readonly FunctionHandle<TReceiver, ArrayView<T>, MemoryViewReceiver<TReceiver, T>> _liftHandle;

        internal ObservableBuilder
        (
            ref TReceiver receiver,
            ref MemoryViewReceiverBuilderObserverHandle<TReceiver, T> observerHandle,
            int initialCapacity,
            FunctionHandle<TReceiver, MemoryViewReceiverBuilderObserverHandle<TReceiver, T>, ArrayViewBuilderObserverHandle<TReceiver, T>> embedHandle,
            FunctionHandle<TReceiver, ArrayView<T>, MemoryViewReceiver<TReceiver, T>> liftHandle
        )
        {
            ArrayViewBuilderObserverHandle<TReceiver, T> embedded = embedHandle.InvokeFunction(ref receiver, ref observerHandle);
            _liftHandle = liftHandle;
            _builder = new(receiver, ref embedded, initialCapacity);
        }

        public void Add(in T source)
        {
            _builder.Add(in source);
        }

        public MemoryViewReceiver<TReceiver, T> Build()
        {
            ArrayView<T> resultArrayView = _builder.Build();
            return _liftHandle.InvokeFunction(ref _builder.RefValue!, ref resultArrayView);
        }
    }
}
