
namespace Nemonuri.LowLevel.DuckTyping;

public struct DuckTypedDynamicMemoryView<TReceiver, T, TInternalMemoryView, TArgument> :
    IMemoryView<T>, 
    IMaybeSupportsRawSpan<T>, 
    IDuckTypeReceiver<TReceiver>,
    IMemoryViewProviderFactory<T, TInternalMemoryView, TReceiver, TArgument>,
    IAddable<T>
    where TInternalMemoryView : IMemoryView<T>, IMaybeSupportsRawSpan<T>
{
    internal MemoryViewDuckTypedProvider<T, TInternalMemoryView, TReceiver, TArgument> _provider;
    private int _length;
    public readonly ActionHandle<MemoryViewDuckTypedProvider<T, TInternalMemoryView, TReceiver, TArgument>, int> Resizer;
    internal TInternalMemoryView _currentMemoryView;

    public DuckTypedDynamicMemoryView
    (
        MemoryViewDuckTypedProvider<T, TInternalMemoryView, TReceiver, TArgument> initialProvider, 
        int initialLength,
        ActionHandle<MemoryViewDuckTypedProvider<T, TInternalMemoryView, TReceiver, TArgument>, int> resizer
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

    [MemberNotNull([nameof(_currentMemoryView)])]
    private void UpdateCurrentMemoryView(in MemoryViewDuckTypedProvider<T, TInternalMemoryView, TReceiver, TArgument> provider)
    {
        _currentMemoryView = provider.InvokeProvider() ?? throw new ArgumentNullException();
    }

    public readonly int Length => _length;

    [UnscopedRef] public ref T this[int index] => ref _currentMemoryView[index];

    public readonly bool SupportsRawSpan => _currentMemoryView.SupportsRawSpan;

    [UnscopedRef] public Span<T> AsSpan => _currentMemoryView.AsSpan;

    [UnscopedRef] public ref readonly TReceiver Receiver => ref _provider.Receiver;

    public readonly MemoryViewDuckTypedProvider<T, TInternalMemoryView, TReceiver, TArgument> ToProvider() => _provider;

    public void Add(in T item)
    {
        EnsureCapacity(Length+1);
        _currentMemoryView[_length] = item;
        _length += 1;
    }
}
