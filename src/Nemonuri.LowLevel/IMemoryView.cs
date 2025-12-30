namespace Nemonuri.LowLevel;

public interface IReadOnlyMemoryView<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    int Length {get;}

    [UnscopedRef] ref readonly T this[int index] {get;}
}

public interface IMemoryView<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    int Length {get;}

    [UnscopedRef] ref T this[int index] {get;}
}

public unsafe readonly struct MemoryViewHandle<TReceiver, T>
{
    private readonly delegate*<in TReceiver, int> _lengthGetter;
    private readonly delegate*<ref TReceiver, int, ref T> _itemGetter;

    public MemoryViewHandle(delegate*<in TReceiver, int> lengthGetter, delegate*<ref TReceiver, int, ref T> itemGetter)
    {
        LowLevelGuard.IsNotNull(lengthGetter);
        LowLevelGuard.IsNotNull(itemGetter);
        
        _lengthGetter = lengthGetter;
        _itemGetter = itemGetter;
    }

    public int GetLength(in TReceiver handler) => _lengthGetter(in handler);
    public ref T GetItem(ref TReceiver handler, int index) => ref _itemGetter(ref handler, index);
}

public struct MemoryViewReceiver<TReceiver, T> : IMemoryView<T>
{
    private TReceiver _receiver;
    private readonly MemoryViewHandle<TReceiver, T> _handle;

    public MemoryViewReceiver(TReceiver receiver, MemoryViewHandle<TReceiver, T> handle)
    {
        _receiver = receiver;
        _handle = handle;
    }

    public readonly int Length => _handle.GetLength(in _receiver);

    [UnscopedRef] public ref T this[int index] => ref _handle.GetItem(ref _receiver, index);
}
