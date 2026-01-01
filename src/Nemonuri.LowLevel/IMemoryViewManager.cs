
namespace Nemonuri.LowLevel;

public interface IMemoryViewManager<TSelf, TKey, TMemoryView> :
    IBuilder<
        LowLevelKeyValuePair<TKey, LowLevelKeyValuePair<TKey, KeyedMemoryViewProviderHandle<TSelf, TKey>>>,
        TMemoryView>
    where TSelf : IMemoryViewManager<TSelf, TKey, TMemoryView>
    where TMemoryView : IMemoryView<LowLevelKeyValuePair<TKey, LowLevelKeyValuePair<TKey, KeyedMemoryViewProviderHandle<TSelf, TKey>>>>
{
}

public readonly struct MemoryViewConfig : 
    IMemoryViewManager<MemoryViewConfig, nint, PackedTableView<nint, LowLevelKeyValuePair<nint, KeyedMemoryViewProviderHandle<MemoryViewConfig, nint>>>>
{
    private readonly PackedTable<nint, LowLevelKeyValuePair<nint, KeyedMemoryViewProviderHandle<MemoryViewConfig, nint>>>.Builder _builder;

    public MemoryViewConfig()
    {
        _builder = PackedTable<nint, LowLevelKeyValuePair<nint, KeyedMemoryViewProviderHandle<MemoryViewConfig, nint>>>.CreateBuilder(4);
    }

    public void Add<T, TMemoryView>(TMemoryView memoryView)
        where TMemoryView : IMemoryView<T>
    #if NET9_0_OR_GREATER
        ,allows ref struct
        where T : allows ref struct
    #endif
    {
        
    }

    public void Add(in LowLevelKeyValuePair<nint, LowLevelKeyValuePair<nint, KeyedMemoryViewProviderHandle<MemoryViewConfig, nint>>> source)
    {
        throw new NotImplementedException();
    }

    public PackedTableView<nint, LowLevelKeyValuePair<nint, KeyedMemoryViewProviderHandle<MemoryViewConfig, nint>>> Build()
    {
        throw new NotImplementedException();
    }
}