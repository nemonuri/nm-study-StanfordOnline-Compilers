namespace Nemonuri.LowLevel;

public interface IPropertyInvokable<T>
{
    [UnscopedRef] ref T? InvokeProperty();
}
