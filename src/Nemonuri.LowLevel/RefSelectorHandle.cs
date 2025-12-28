
namespace Nemonuri.LowLevel;

public unsafe readonly struct ReadOnlyRefSelectorHandle<TSource, TResult>
{
    private readonly delegate*<ref readonly TSource, ref readonly TResult> _selector;

    public ReadOnlyRefSelectorHandle(delegate*<ref readonly TSource, ref readonly TResult> selector)
    {
        LowLevelGuard.IsNotNull(selector);
        _selector = selector;
    }

#pragma warning disable CS9088
    public ref readonly TResult Invoke(scoped ref readonly TSource source) => ref _selector(in source);
#pragma warning restore CS9088
}

public unsafe readonly struct RefSelectorHandle<TSource, TResult>
{
    private readonly delegate*<ref TSource, ref TResult> _selector;

    public RefSelectorHandle(delegate*<ref TSource, ref TResult> selector)
    {
        LowLevelGuard.IsNotNull(selector);
        _selector = selector;
    }

    public ref TResult Invoke(ref TSource source) => ref _selector(ref source);
}

public unsafe readonly struct RefLikeSelectorHandle<THandler, TSource, TResult>
{
    private readonly delegate*<in THandler, ref TSource, ref TResult> _selector;

    public RefLikeSelectorHandle(delegate*<in THandler, ref TSource, ref TResult> selector)
    {
        LowLevelGuard.IsNotNull(selector);
        _selector = selector;
    }

    public ref TResult Invoke(in THandler handler, ref TSource source) => ref _selector(in handler, ref source);
}
