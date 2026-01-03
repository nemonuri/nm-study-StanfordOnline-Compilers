namespace Nemonuri.LowLevel;

public interface IPropertyInvokable<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    [UnscopedRef] ref T? InvokeProperty();
}

public interface IProviderInvokable<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    [UnscopedRef] ref readonly T? InvokeProvider();
}
