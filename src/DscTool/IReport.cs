namespace DscTool;

public interface IReport<TDesired, TDiagnostic>
{
    public bool TryGetDesired([NotNullWhen(true)] out TDesired? desired);
    public ReadOnlySpan<TDiagnostic> Diagnostics {get;}
}
