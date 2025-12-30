namespace Nemonuri.LowLevel;

public interface IRefBox<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    [UnscopedRef] ref T? Value {get;}
}

public unsafe readonly struct RefBoxHandle<TReceiver, T>
#if NET9_0_OR_GREATER
    where TReceiver : allows ref struct
    where T : allows ref struct
#endif
{
    private readonly delegate*<ref TReceiver, ref T?> _valueGetter;

    public RefBoxHandle(delegate*<ref TReceiver, ref T?> valueGetter)
    {
        _valueGetter = valueGetter;
    }

    public ref T? GetValueRef(ref TReceiver receiver) => ref _valueGetter(ref receiver);
}

public struct BoxableRefBoxReceiver<TReceiver, T> : IRefBox<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    private TReceiver _receiver;
    private readonly RefBoxHandle<TReceiver, T> _handle;

    public BoxableRefBoxReceiver(TReceiver receiver, RefBoxHandle<TReceiver, T> handle)
    {
        _receiver = receiver;
        _handle = handle;
    }

    [UnscopedRef] public ref T? Value => ref _handle.GetValueRef(ref _receiver);
}

#if NET9_0_OR_GREATER
public ref struct RefBoxReceiver<TReceiver, T> : IRefBox<T>
    where T : allows ref struct
    where TReceiver : allows ref struct
{
    private TReceiver _receiver;
    private readonly RefBoxHandle<TReceiver, T> _handle;

    public RefBoxReceiver(TReceiver receiver, RefBoxHandle<TReceiver, T> handle)
    {
        _receiver = receiver;
        _handle = handle;
    }

    [UnscopedRef] public ref T? Value => ref _handle.GetValueRef(ref _receiver);
}
#endif