
namespace Nemonuri.LowLevel;

public interface ILowLevelComparer<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    int Compare(scoped in T? left, scoped in T? right);
}

public unsafe readonly struct LowLevelComparerHandle<T> : ILowLevelComparer<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    private readonly delegate*<in T?, in T?, int> _comparer;

    public LowLevelComparerHandle(delegate*<in T?, in T?, int> comparer)
    {
        LowLevelGuard.IsNotNull(comparer);
        _comparer = comparer;
    }

    public int Compare(scoped in T? left, scoped in T? right) => _comparer(in left, in right);
}

public unsafe readonly struct LowLevelComparerHandle<TReceiver, T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    private readonly delegate*<in TReceiver, in T?, in T?, int> _comparer;

    public LowLevelComparerHandle(delegate*<in TReceiver, in T?, in T?, int> comparer)
    {
        LowLevelGuard.IsNotNull(comparer);
        _comparer = comparer;
    }

    public int Compare(scoped in TReceiver receiver, scoped in T? left, scoped in T? right) => _comparer(in receiver, in left, in right);
}

public readonly struct LowLevelComparerReceiver<TReceiver, T> : ILowLevelComparer<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    private readonly TReceiver _receiver;
    private readonly LowLevelComparerHandle<TReceiver, T> _handle;

    public LowLevelComparerReceiver(TReceiver receiver, LowLevelComparerHandle<TReceiver, T> handle)
    {
        _receiver = receiver;
        _handle = handle;
    }

    public int Compare(scoped in T? left, scoped in T? right) => _handle.Compare(in _receiver, in left, in right);
}