namespace Nemonuri.LowLevel;

public interface IPropertyInvokable<T>
{
    [UnscopedRef] ref T? InvokeProperty();
}

public interface IProviderInvokable<T>
{
    [UnscopedRef] ref readonly T? InvokeProvider();
}
