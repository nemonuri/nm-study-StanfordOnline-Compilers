namespace DscTool;

public delegate TResult ReadOnlySpanFunc<T, TResult>(ReadOnlySpan<T> span);

public delegate TReport ReportFactory<TDesired, TDiagnostic, TReport>(TDesired? desired1, TDesired? desired2, ReadOnlySpan<TDiagnostic> diagnostics)
    where TReport : IReport<TDesired, TDiagnostic>;