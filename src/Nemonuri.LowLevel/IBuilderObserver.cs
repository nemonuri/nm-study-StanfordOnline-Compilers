namespace Nemonuri.LowLevel;

public interface IBuilderObserver<TSource, TResult, TBuilder>
    where TBuilder : IBuilder<TSource, TResult>
{
    void OnBuilderCreated(scoped in TBuilder builder);
    void OnAdding(scoped in TBuilder builder, scoped in TSource source);
    void OnAdded(scoped in TBuilder builder);
    void OnBuilding(scoped in TBuilder builder);
    void OnBuilt(scoped in TBuilder builder, scoped in TResult buildResult);
}

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct BuilderObserverHandle<TReceiver, TSource, TResult, TBuilder>
    where TBuilder : IBuilder<TSource, TResult>
{
    private readonly delegate*<ref TReceiver, in TBuilder, void> _pOnBuilderCreated;
    private readonly delegate*<ref TReceiver, in TBuilder, in TSource, void> _pOnAdding;
    private readonly delegate*<ref TReceiver, in TBuilder, void> _pOnAdded;
    private readonly delegate*<ref TReceiver, in TBuilder, void> _pOnBuilding;
    private readonly delegate*<ref TReceiver, in TBuilder, in TResult, void> _pOnBuilt;

    public BuilderObserverHandle
    (
        delegate*<ref TReceiver, in TBuilder, void> pOnBuilderCreated,
        delegate*<ref TReceiver, in TBuilder, in TSource, void> pOnAdding,
        delegate*<ref TReceiver, in TBuilder, void> pOnAdded,
        delegate*<ref TReceiver, in TBuilder, void> pOnBuilding,
        delegate*<ref TReceiver, in TBuilder, in TResult, void> pOnBuilt
    )
    {
        _pOnBuilderCreated = pOnBuilderCreated;
        _pOnAdding = pOnAdding;
        _pOnAdded = pOnAdded;
        _pOnBuilding = pOnBuilding;
        _pOnBuilt = pOnBuilt;
    }

    public void InvokeOnBuilderCreated(ref TReceiver receiver, scoped in TBuilder builder)
    {
        if (_pOnBuilderCreated != null) _pOnBuilderCreated(ref receiver, in builder);
    }

    public void InvokeOnAdding(ref TReceiver receiver, scoped in TBuilder builder, scoped in TSource source)
    {
        if (_pOnAdding != null) _pOnAdding(ref receiver, in builder, in source);
    }

    public void InvokeOnAdded(ref TReceiver receiver, scoped in TBuilder builder)
    {
        if (_pOnAdded != null) _pOnAdded(ref receiver, in builder);
    }

    public void InvokeOnBuilding(ref TReceiver receiver, scoped in TBuilder builder)
    {
        if (_pOnBuilding != null) _pOnBuilding(ref receiver, in builder);
    }

    public void InvokeOnBuilded(ref TReceiver receiver, scoped in TBuilder builder, scoped in TResult buildResult)
    {
        if (_pOnBuilt != null) _pOnBuilt(ref receiver, in builder, in buildResult);
    }
}
