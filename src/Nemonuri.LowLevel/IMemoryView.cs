namespace Nemonuri.LowLevel;

public interface IMemoryView<TView>
{
    int Length {get;}

    [UnscopedRef] ref TView this[int index] {get;}
}

public unsafe readonly struct MemoryViewHandle<THandler, TView>
{
    private readonly delegate*<in THandler, int> _lengthGetter;
    private readonly delegate*<in THandler, int, ref TView> _itemGetter;

    public MemoryViewHandle(delegate*<in THandler, int> lengthGetter, delegate*<in THandler, int, ref TView> itemGetter)
    {
        LowLevelGuard.IsNotNull(lengthGetter);
        LowLevelGuard.IsNotNull(itemGetter);
        
        _lengthGetter = lengthGetter;
        _itemGetter = itemGetter;
    }

    public int GetLength(in THandler handler) => _lengthGetter(in handler);
    public ref TView GetItem(in THandler handler, int index) => ref _itemGetter(in handler, index);
}

public readonly struct AdHocMemoryView<THandler, TView> : IMemoryView<TView>
{
    private readonly THandler _handler;
    private readonly MemoryViewHandle<THandler, TView> _handle;

    public AdHocMemoryView(THandler handler, MemoryViewHandle<THandler, TView> handle)
    {
        _handler = handler;
        _handle = handle;
    }

    public int Length => _handle.GetLength(in _handler);

    [UnscopedRef] public ref TView this[int index] => ref _handle.GetItem(in _handler, index);
}