
namespace Nemonuri.LowLevel;

public interface ILowLevelComparer<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    int Compare(scoped in T left, scoped in T right);
}

public unsafe readonly struct LowLevelComparerHandle<T> : ILowLevelComparer<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    private readonly delegate*<in T, in T, int> _comparer;

    public LowLevelComparerHandle(delegate*<in T, in T, int> comparer)
    {
        LowLevelGuard.IsNotNull(comparer);
        _comparer = comparer;
    }

    public int Compare(scoped in T left, scoped in T right) => _comparer(in left, in right);
}