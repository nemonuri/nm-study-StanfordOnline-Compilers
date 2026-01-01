// Reference: https://github.com/dotnet/runtime/blob/main/src/coreclr/tools/Common/System/Collections/Generic/ArrayBuilder.cs

namespace Nemonuri.LowLevel;

public struct ArrayViewObservableBuilder<T, TReceiver> : 
    IMemoryView<T>, 
    IMaybeSupportsRawSpan<T>, 
    IBuilder<T, ArrayView<T>>,
    IRefBox<TReceiver>
{
    private TReceiver _receiver;
    private BuilderObserverHandle<TReceiver, T, ArrayView<T>, ArrayViewObservableBuilder<T, TReceiver>> _observerHandle;
    private ArrayViewBuilder<T> _builder;

    public ArrayViewObservableBuilder
    (
        TReceiver receiver, 
        ref ArrayViewBuilderObserverHandle<TReceiver, T> observerHandle,
        int initialCapacity
    )
    {
        _receiver = receiver;
        _observerHandle = observerHandle.Unwrap();
        _builder = new(initialCapacity);
        _observerHandle.InvokeOnBuilderCreated(ref _receiver, in this);
    }

    public int Length => _builder.Length;

    public ref T this[int index] => ref _builder[index];

    public bool SupportsRawSpan => _builder.SupportsRawSpan;

    public Span<T> AsSpan => _builder.AsSpan;

    public void Add(in T source)
    {
        _observerHandle.InvokeOnAdding(ref _receiver, in this, in source);
        _builder.Add(in source);
        _observerHandle.InvokeOnAdded(ref _receiver, in this);
    }

    public ArrayView<T> Build()
    {
        _observerHandle.InvokeOnBuilding(ref _receiver, in this);
        var result = _builder.Build();
        _observerHandle.InvokeOnBuilded(ref _receiver, in this, in result);
        return result;
    }

    [UnscopedRef]
    public ref TReceiver? RefValue => ref _receiver!;
}
