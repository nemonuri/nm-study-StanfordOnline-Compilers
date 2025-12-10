namespace DscTool;

internal readonly struct ThunkedResourceStateTester<TDiagnostic, TReport>
    where TReport : IReport<bool, TDiagnostic>
{
    private readonly Func<IState, IState, TReport> _impl;
    internal ThunkedResourceStateTester(Func<IState, IState, TReport> impl) 
    {
        _impl = impl;
    }

    public TReport TestState(IState left, IState right) => _impl(left, right);
}

internal static class ThunkedResourceStateTesterDictionary<TDiagnostic, TReport>
    where TReport : IReport<bool, TDiagnostic>
{
    private static volatile ConcurrentDictionary<Type, ThunkedResourceStateTester<TDiagnostic, TReport>>? s_singleton = null;

    public static ConcurrentDictionary<Type, ThunkedResourceStateTester<TDiagnostic, TReport>> Singleton => 
        s_singleton ??= Interlocked.CompareExchange(ref s_singleton, new(), null) ?? s_singleton;
    
    public static void Delete()
    {
        var toClear = Interlocked.Exchange(ref s_singleton, null);
        toClear?.Clear();
    }
}

internal static class ThunkedResourceStateTester
{
    extension<TResourceState, TDiagnostic, TReport>(ThunkedResourceStateTester<TDiagnostic, TReport>)
        where TReport : IReport<bool, TDiagnostic>
    {
        public static ThunkedResourceStateTester<TDiagnostic, TReport> Thunk(IResourceStateTester<TResourceState, TDiagnostic, TReport> tester)
        {
            Guard.IsNotNull(tester);
            return new ThunkedResourceStateTester<TDiagnostic, TReport>(Impl);

            TReport Impl(IState desired, IState current)
            {
                if (!(desired is TResourceState ensuredDesired && current is TResourceState ensuredCurrent))
                {
                    return ThrowHelper.ThrowArgumentException<TReport>($"Not expected type: Expected = {typeof(TResourceState)}");
                }
                return tester.TestState(ensuredDesired, ensuredCurrent);
            }
        }

        public static bool TryGetFromCache(out ThunkedResourceStateTester<TDiagnostic, TReport> tester) => 
            ThunkedResourceStateTesterDictionary<TDiagnostic, TReport>.Singleton.TryGetValue(typeof(TResourceState), out tester);

        public static ThunkedResourceStateTester<TDiagnostic, TReport> GetOrAddFromCache(IResourceStateTester<TResourceState, TDiagnostic, TReport> tester) =>
            ThunkedResourceStateTesterDictionary<TDiagnostic, TReport>.Singleton.GetOrAdd(typeof(TResourceState), Thunk(tester));
    }

    extension<TDiagnostic, TReport>(ThunkedResourceStateTester<TDiagnostic, TReport>)
        where TReport : IReport<bool, TDiagnostic>
    {
        public static void DeleteCache() => ThunkedResourceStateTesterDictionary<TDiagnostic, TReport>.Delete();
    }
}
