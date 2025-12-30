
namespace Nemonuri.LowLevel;

public interface IMemoryViewProvider<TView, TMemoryView>
    where TMemoryView : IMemoryView<TView>
#if NET9_0_OR_GREATER
    ,allows ref struct
#endif
{
    void GetMemoryView(scoped ref TMemoryView memoryView);
}

public unsafe readonly struct MemoryViewProviderHandle<TReceiver, T, TMemoryView>
#if NET9_0_OR_GREATER
    where TMemoryView : allows ref struct
#endif
{
    private readonly delegate*<ref TReceiver, out TMemoryView, void> _getMemoryViewImpl;

    public MemoryViewProviderHandle(delegate*<ref TReceiver, out TMemoryView, void> getMemoryViewImpl)
    {
        LowLevelGuard.IsNotNull(getMemoryViewImpl);
        _getMemoryViewImpl = getMemoryViewImpl;
    }

#pragma warning disable CS9094
    public void GetMemoryView(ref TReceiver handler, scoped ref TMemoryView memoryView) => 
        _getMemoryViewImpl(ref handler, out memoryView);
#pragma warning restore CS9094
}

public struct MemoryViewProviderReceiver<TReceiver, TView, TMemoryView> :
    IMemoryViewProvider<TView, TMemoryView>
    where TMemoryView : IMemoryView<TView>
#if NET9_0_OR_GREATER
    ,allows ref struct
#endif
{
    private TReceiver _receiver;
    private readonly MemoryViewProviderHandle<TReceiver, TView, TMemoryView> _handle;

    public MemoryViewProviderReceiver(TReceiver receiver, MemoryViewProviderHandle<TReceiver, TView, TMemoryView> handle)
    {
        _receiver = receiver;
        _handle = handle;
    }

    public void GetMemoryView(scoped ref TMemoryView memoryView) => _handle.GetMemoryView(ref _receiver, ref memoryView);
}