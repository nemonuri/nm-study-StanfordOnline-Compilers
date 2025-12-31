using System.Collections.Immutable;

namespace Nemonuri.LowLevel;

public readonly struct MemoryView<T, TView> : IMemoryView<TView>
{
    private readonly Memory<T> _memory;
    private readonly RefSelectorHandle<T, TView> _selectorHandle;

    public MemoryView(Memory<T> memory, RefSelectorHandle<T, TView> selectorHandle)
    {
        _memory = memory;
        _selectorHandle = selectorHandle;
    }

    public int Length => _memory.Length;

    public ref TView this[int index] => 
        ref _selectorHandle.Invoke(ref _memory.Span[index]);
}

public readonly struct ArraySegmentView<T, TView> : IMemoryView<TView>
{
    private readonly ArraySegment<T> _arraySegment;
    private readonly RefSelectorHandle<T, TView> _selectorHandle;

    public ArraySegmentView(ArraySegment<T> arraySegment, RefSelectorHandle<T, TView> selectorHandle)
    {
        _arraySegment = arraySegment;
        _selectorHandle = selectorHandle;
    }

    public int Length => _arraySegment.Count;

    private ref T GetRef(int index)
    {
        return ref (_arraySegment.Array ?? [])[_arraySegment.Offset + index];
    }

    public ref TView this[int index] => ref _selectorHandle.Invoke(ref GetRef(index));
}

public readonly struct ArrayView<T> : IMemoryView<T>, IMaybeSupportsRawSpan<T>
{
    private readonly T[] _values;

    public ArrayView(T[] values)
    {
        _values = values;
    }

    public int Length => _values.Length;

    public ref T this[int index] => ref _values[index];

    public bool SupportsRawSpan => true;

    public Span<T> AsSpan => _values.AsSpan();
}

public struct ArrayView<TSource, TResult> : IMemoryView<TResult>
{
    private MappedView<TSource, ArrayView<TSource>, TResult> _mappedView;

    public ArrayView(ArrayView<TSource> sourceView, RefSelectorHandle<TSource, TResult> selectorHandle)
    {
        _mappedView = new(sourceView, selectorHandle);
    }

    public int Length => _mappedView.Length;

    [UnscopedRef] public ref TResult this[int index] => ref _mappedView[index];
}


public readonly struct ImmutableArrayView<T> : IReadOnlyMemoryView<T>
{
    private readonly ImmutableArray<T> _array;

    public ImmutableArrayView(ImmutableArray<T> array)
    {
        _array = array;
    }

    public int Length => _array.Length;

    public ref readonly T this[int index] => ref (ImmutableCollectionsMarshal.AsArray(_array) ?? [])[index];
}

public readonly struct ImmutableArrayView<TSource, TResult> : IReadOnlyMemoryView<TResult>
{
    private readonly ReadOnlyMappedView<TSource, ImmutableArrayView<TSource>, TResult> _mappedView;

    public ImmutableArrayView(ImmutableArrayView<TSource> sourceView, ReadOnlyRefSelectorHandle<TSource, TResult> selectorHandle)
    {
        _mappedView = new(sourceView, selectorHandle);
    }

    public int Length => _mappedView.Length;

    [UnscopedRef] public ref readonly TResult this[int index] => ref _mappedView[index];
}
