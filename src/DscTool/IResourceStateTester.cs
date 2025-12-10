namespace DscTool;

public interface IResourceStateTester<TResourceState, TDiagnostic, TReport>
    where TReport : IReport<bool, TDiagnostic>
{
    TReport TestState(TResourceState desired, TResourceState current);
}
