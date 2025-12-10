namespace DscTool.Extensions;

using static DscTool.ResultTagger;

public static class DesiredStatePremise
{
    public struct TTestDesiredStateResult {/*TODO*/}

    extension
    <TResourceDescriptor, 
     TState, TDiagnostic, TReport,
     TStateComparerDiagnostic, TStateComparerReport, TStateComparer,
     TDesiredStatePremise>
    (TDesiredStatePremise premise)
    where TReport : IReport<TState, TDiagnostic>
    where TStateComparerReport : IReport<TState, TStateComparerDiagnostic>
    where TStateComparer : IResourceStateTester<TState, TStateComparerDiagnostic, TStateComparerReport>
    where TDesiredStatePremise : IDesiredStatePremise
    <TResourceDescriptor, 
     TState, TDiagnostic, TReport,
     TStateComparerDiagnostic, TStateComparerReport, TStateComparer>
    {
        public Result<SimpleTestResult<TState>, > TestResourceSimple(TResourceDescriptor resource)
        {
            TReport report = premise.RequestCurrentState(resource);
            if (report.IsSuccess<TState, TDiagnostic, TReport>())
            {
                
            }
            /*
            .Match<TState, TDiagnostic, TReport, bool>
            (
                caseOfSussess: static _ => true, 
                caseOfFail: static _ => true,
                diagnosticCondition: null,
                ensureEmptyLikeDiagnostics: true
            );
            */
        }
    }
}
