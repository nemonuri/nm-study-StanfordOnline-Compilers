namespace Nemonuri.LowLevel;

public interface IBuilder<TSource, TResult>
{
    void Add(in TSource source);
    TResult Build();
}

public unsafe readonly struct BuilderHandle<TReceiver, TSource, TResult>
{
    private readonly delegate*<ref TReceiver, in TSource, void> _pAdd;
    private readonly delegate*<ref TReceiver, TResult> _pBuild;

    public BuilderHandle(delegate*<ref TReceiver, in TSource, void> pAdd, delegate*<ref TReceiver, TResult> pBuild)
    {
        LowLevelGuard.IsNotNull(pAdd);
        LowLevelGuard.IsNotNull(pBuild);

        _pAdd = pAdd;
        _pBuild = pBuild;
    }

    public void InvokeAdd(ref TReceiver receiver, in TSource source) => _pAdd(ref receiver, in source);
    public TResult InvokeBuild(ref TReceiver receiver) => _pBuild(ref receiver);
}

public struct BuilderReceiver<TReceiver, TSource, TResult> : IBuilder<TSource, TResult>
{
    private TReceiver _receiver;
    private readonly BuilderHandle<TReceiver, TSource, TResult> _handle;

    public BuilderReceiver(TReceiver receiver, BuilderHandle<TReceiver, TSource, TResult> handle)
    {
        _receiver = receiver;
        _handle = handle;
    }

    public void Add(in TSource source) => _handle.InvokeAdd(ref _receiver, in source);

    public TResult Build() => _handle.InvokeBuild(ref _receiver);
}