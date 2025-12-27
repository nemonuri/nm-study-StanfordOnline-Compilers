
namespace Nemonuri.LowLevel;

public unsafe readonly struct RefSelectorHandle<TSource, TResult>
{
    private readonly delegate*<ref TSource, ref TResult> _selector;

    public RefSelectorHandle(delegate*<ref TSource, ref TResult> selector)
    {
        Guard.IsTrue(selector is not null);
        _selector = selector;
    }

    public ref TResult Invoke(ref TSource source) => ref _selector(ref source);
}
