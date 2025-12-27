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

public readonly struct ArrayView<T, TView> : IMemoryView<TView>
{
    private readonly T[] _values;
    private readonly RefSelectorHandle<T, TView> _selectorHandle;

    public ArrayView(T[] values, RefSelectorHandle<T, TView> selectorHandle)
    {
        _values = values;
        _selectorHandle = selectorHandle;
    }

    public int Length => _values.Length;

    public ref TView this[int index] => ref _selectorHandle.Invoke(ref _values[index]);
}
