namespace DscTool;

public interface IDiagnosticCondition<TDiagnostic>
{
    bool CheckDiagnostic(TDiagnostic diagnostic);
}

public readonly struct AdHocDiagnosticCondition<TDiagnostic>(Func<TDiagnostic, bool>? impl)
    : IDiagnosticCondition<TDiagnostic>
{
    private readonly Func<TDiagnostic, bool>? _impl = impl;

    public bool CheckDiagnostic(TDiagnostic diagnostic) => _impl?.Invoke(diagnostic) ?? true;
}
