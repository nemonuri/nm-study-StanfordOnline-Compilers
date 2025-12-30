namespace Nemonuri.LowLevel;

public interface IBox<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    T? Value {[UnscopedRef] get;set;}
}

public unsafe readonly struct BoxHandle<TReceiver, T>
#if NET9_0_OR_GREATER
    where TReceiver : allows ref struct
    where T : allows ref struct
#endif
{
    private readonly delegate*<ref TReceiver, T?> _valueGetter;
    private readonly delegate*<ref TReceiver, in T?, void> _valueSetter;

    public BoxHandle(delegate*<ref TReceiver, T?> valueGetter, delegate*<ref TReceiver, in T?, void> valueSetter)
    {
        LowLevelGuard.IsNotNull(valueGetter);
        LowLevelGuard.IsNotNull(valueSetter);

        _valueGetter = valueGetter;
        _valueSetter = valueSetter;
    }

    public T? GetValue(ref TReceiver receiver) => _valueGetter(ref receiver);
    public void SetValue(ref TReceiver receiver, in T? value) => _valueSetter(ref receiver, in value);
}

public struct BoxReceiver<TReceiver, T> : IBox<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    private TReceiver _receiver;
    private readonly BoxHandle<TReceiver, T> _handle;

    public BoxReceiver(TReceiver receiver, BoxHandle<TReceiver, T> handle)
    {
        _receiver = receiver;
        _handle = handle;
    }

    public T? Value 
    { 
        [UnscopedRef]
        get => _handle.GetValue(ref _receiver); 
        set => _handle.SetValue(ref _receiver, in value);
    }
}


