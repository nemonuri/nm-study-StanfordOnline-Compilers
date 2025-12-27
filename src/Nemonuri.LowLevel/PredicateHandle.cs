
namespace Nemonuri.LowLevel;

public unsafe readonly struct PredicateHandle<T1, T2>
{
    private readonly delegate*<in T1?, in T2?, bool> _predicate;

    public PredicateHandle(delegate*<in T1?, in T2?, bool> predicate)
    {
        Guard.IsTrue(predicate is not null);
        _predicate = predicate;
    }

    public bool Invoke(scoped in T1? arg1, scoped in T2? arg2) => _predicate(in arg1, in arg2);
}
