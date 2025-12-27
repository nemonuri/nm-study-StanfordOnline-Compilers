namespace Nemonuri.LowLevel;

public interface IMemoryView<TView>
{
    int Length {get;}

    [UnscopedRef] ref TView this[int index] {get;}
}

public unsafe readonly struct MemoryViewHandle<THandler, TView>
{
    private readonly delegate*<in THandler, int> _lengthGetter;
    private readonly delegate*<ref THandler, int, ref TView> _itemGetter;

    public MemoryViewHandle(delegate*<in THandler, int> lengthGetter, delegate*<ref THandler, int, ref TView> itemGetter)
    {
        LowLevelGuard.IsNotNull(lengthGetter);
        LowLevelGuard.IsNotNull(itemGetter);
        
        _lengthGetter = lengthGetter;
        _itemGetter = itemGetter;
    }

    public int GetLength(in THandler handler) => _lengthGetter(in handler);
    public ref TView GetItem(ref THandler handler, int index) => ref _itemGetter(ref handler, index);
}

public struct LowLevelAbstractMemoryView<THandler, TView> : IMemoryView<TView>
{
    private THandler _handler;
    private readonly MemoryViewHandle<THandler, TView> _handle;

    public LowLevelAbstractMemoryView(THandler handler, MemoryViewHandle<THandler, TView> handle)
    {
        _handler = handler;
        _handle = handle;
    }

    public readonly int Length => _handle.GetLength(in _handler);

    [UnscopedRef] public ref TView this[int index] => ref _handle.GetItem(ref _handler, index);
}
