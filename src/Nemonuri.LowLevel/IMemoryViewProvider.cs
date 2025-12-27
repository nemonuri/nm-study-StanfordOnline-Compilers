
namespace Nemonuri.LowLevel;

public interface IMemoryViewProvider<TView, TMemoryView>
    where TMemoryView : IMemoryView<TView>
#if NET9_0_OR_GREATER
    ,allows ref struct
#endif
{
    void GetMemoryView(scoped ref TMemoryView memoryView);
}

public unsafe readonly struct AbstractMemoryViewProviderHandle<THandler, TView, TMemoryView>
#if NET9_0_OR_GREATER
    where TMemoryView : allows ref struct
#endif
{
    private readonly delegate*<ref THandler, out TMemoryView, void> _getMemoryViewImpl;

    public AbstractMemoryViewProviderHandle(delegate*<ref THandler, out TMemoryView, void> getMemoryViewImpl)
    {
        LowLevelGuard.IsNotNull(getMemoryViewImpl);
        _getMemoryViewImpl = getMemoryViewImpl;
    }

#pragma warning disable CS9094
    public void GetMemoryView(ref THandler handler, scoped ref TMemoryView memoryView) => 
        _getMemoryViewImpl(ref handler, out memoryView);
#pragma warning restore CS9094
}

public struct AbstractMemoryViewProvider<THandler, TView, TMemoryView> :
    IMemoryViewProvider<TView, TMemoryView>
    where TMemoryView : IMemoryView<TView>
#if NET9_0_OR_GREATER
    ,allows ref struct
#endif
{
    private THandler _handler;
    private readonly AbstractMemoryViewProviderHandle<THandler, TView, TMemoryView> _handle;

    public AbstractMemoryViewProvider(THandler handler, AbstractMemoryViewProviderHandle<THandler, TView, TMemoryView> handle)
    {
        _handler = handler;
        _handle = handle;
    }

    public void GetMemoryView(scoped ref TMemoryView memoryView) => _handle.GetMemoryView(ref _handler, ref memoryView);
}