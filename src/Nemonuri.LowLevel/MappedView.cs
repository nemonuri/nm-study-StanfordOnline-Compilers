namespace Nemonuri.LowLevel;

public struct MappedView<TSource, TSourceView, TResult> : IMemoryView<TResult>
    where TSourceView : IMemoryView<TSource>
{
    private TSourceView _sourceView;
    private readonly RefSelectorHandle<TSource, TResult> _selectorHandle;

    public MappedView(TSourceView sourceView, RefSelectorHandle<TSource, TResult> selectorHandle)
    {
        _sourceView = sourceView;
        _selectorHandle = selectorHandle;
    }

    public int Length => _sourceView.Length;

    [UnscopedRef] public ref TResult this[int index] => ref _selectorHandle.Invoke(ref _sourceView[index]);
}

public readonly struct ReadOnlyMappedView<TSource, TSourceView, TResult> : IReadOnlyMemoryView<TResult>
    where TSourceView : IReadOnlyMemoryView<TSource>
{
    private readonly TSourceView _sourceView;
    private readonly ReadOnlyRefSelectorHandle<TSource, TResult> _selectorHandle;

    public ReadOnlyMappedView(TSourceView sourceView, ReadOnlyRefSelectorHandle<TSource, TResult> selectorHandle)
    {
        _sourceView = sourceView;
        _selectorHandle = selectorHandle;
    }

    public int Length => _sourceView.Length;

    [UnscopedRef] public ref readonly TResult this[int index] => ref _selectorHandle.Invoke(in _sourceView[index]);
}