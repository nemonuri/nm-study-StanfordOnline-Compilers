namespace Nemonuri.LowLevel;

public interface IReadOnlyBox<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    T? Value {[UnscopedRef] get;}
}

public unsafe readonly struct ReadOnlyBoxHandle<TReceiver, T>
#if NET9_0_OR_GREATER
    where TReceiver : allows ref struct
    where T : allows ref struct
#endif
{
    private readonly delegate*<ref TReceiver, T?> _valueGetter;

    public ReadOnlyBoxHandle(delegate*<ref TReceiver, T?> valueGetter)
    {
        LowLevelGuard.IsNotNull(valueGetter);

        _valueGetter = valueGetter;
    }

    public T? GetValue(ref TReceiver receiver) => _valueGetter(ref receiver);
}

public struct ReadOnlyBoxReceiver<TReceiver, T> : IReadOnlyBox<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    private TReceiver _receiver;
    private readonly ReadOnlyBoxHandle<TReceiver, T> _handle;

    public ReadOnlyBoxReceiver(TReceiver receiver, ReadOnlyBoxHandle<TReceiver, T> handle)
    {
        _receiver = receiver;
        _handle = handle;
    }

    public T? Value 
    { 
        [UnscopedRef]
        get => _handle.GetValue(ref _receiver);
    }
}
