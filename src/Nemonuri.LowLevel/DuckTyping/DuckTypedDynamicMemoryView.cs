
namespace Nemonuri.LowLevel.DuckTyping;

public struct DuckTypedDynamicMemoryView<TReceiver, T, TArgument> :
    IMemoryView<T>, 
    IMaybeSupportsRawSpan<T>, 
    IDuckTypeReceiver<TReceiver>,
    IMemoryViewProviderFactory<T, DuckTypedMemoryView<TReceiver, T>, TReceiver, TArgument>,
    IAddable<T>
{
    private MemoryViewDuckTypedProvider<T, DuckTypedMemoryView<TReceiver, T>, TReceiver, TArgument> _provider;
    private int _length;
    public readonly ActionHandle<MemoryViewDuckTypedProvider<T, DuckTypedMemoryView<TReceiver, T>, TReceiver, TArgument>, int> Resizer;
    private DuckTypedMemoryView<TReceiver, T> _currentMemoryView;

    public DuckTypedDynamicMemoryView
    (
        MemoryViewDuckTypedProvider<T, DuckTypedMemoryView<TReceiver, T>, TReceiver, TArgument> initialProvider, 
        int initialLength,
        ActionHandle<MemoryViewDuckTypedProvider<T, DuckTypedMemoryView<TReceiver, T>, TReceiver, TArgument>, int> resizer
    )
    {
        _provider = initialProvider;
        _length = initialLength;
        Resizer = resizer;
        UpdateCurrentMemoryView(in _provider);

        Guard.IsGreaterThanOrEqualTo(initialLength, 0);
        EnsureCapacity(initialLength);
    }

    private readonly int Capacity => _currentMemoryView.Length;

    private void EnsureCapacity(int requestedCapacity)
    {
        if (!(requestedCapacity <= Capacity))
        {
            Grow(requestedCapacity);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int requestedCapacity)
    {
        int newCapacity = Math.Max(2 * Capacity + 1, requestedCapacity);
        Resizer.InvokeAction(ref _provider, in newCapacity);
        UpdateCurrentMemoryView(in _provider);
    }

    private void UpdateCurrentMemoryView(in MemoryViewDuckTypedProvider<T, DuckTypedMemoryView<TReceiver, T>, TReceiver, TArgument> provider)
    {
        _currentMemoryView = provider.InvokeProvider();
    }

    public readonly int Length => _length;

    [UnscopedRef] public ref T this[int index] => ref _currentMemoryView[index];

    public readonly bool SupportsRawSpan => _currentMemoryView.SupportsRawSpan;

    [UnscopedRef] public Span<T> AsSpan => _currentMemoryView.AsSpan;

    [UnscopedRef] public ref readonly TReceiver Receiver => ref _provider.Receiver;

    public readonly MemoryViewDuckTypedProvider<T, DuckTypedMemoryView<TReceiver, T>, TReceiver, TArgument> ToProvider() => _provider;

    public void Add(in T item)
    {
        EnsureCapacity(Length+1);
        _currentMemoryView[_length] = item;
        _length += 1;
    }
}
