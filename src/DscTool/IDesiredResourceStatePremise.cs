namespace DscTool;

public interface IDesiredResourceStatePremise
    <TResource, TResourcePremise, TState, TStatePremise, TDiagnostic, TStateReport, TStateReportPremise, TTestReport> :
    IDiagnosticCondition<TDiagnostic>
    where TResourcePremise : IUnionPremise<TResource>, ITreePremise<TResource>
    where TState : IState<TResource>
    where TStatePremise : IUnionPremise<TState>, ITreePremise<TState>
    where TStateReport : IReport<TState, TDiagnostic>
    where TStateReportPremise : IUnionPremise<TStateReport>
    where TTestReport : IReport<bool, TDiagnostic>
{
    ref readonly TResourcePremise ResourcePremise {get;}

    ref readonly TStatePremise StatePremise {get;}

    ref readonly TStateReportPremise StateReportPremise {get;}

    ref readonly TSubStateReport RequestDesiredState<
        [Liftable(nameof(StatePremise), nameof(TState))] TSubState, 
        [Liftable(nameof(StateReportPremise), nameof(TStateReport))] TSubStateReport
        >(scoped ref readonly TResource resource)
        ;

    TSubStateReport RequestStateSnapshot<
        [Liftable(nameof(StatePremise), nameof(TState))] TSubState, 
        [Liftable(nameof(StateReportPremise), nameof(TStateReport))] TSubStateReport
        >(scoped ref readonly TResource resource)
        ;

    TTestReport TestState<
        [Liftable(nameof(StatePremise), nameof(TState))] TSubState, 
        [Liftable(nameof(StateReportPremise), nameof(TStateReport))] TSubStateReport
        >(TSubState desiredState, TSubState stateSnapshot)
        ;

    TSubStateReport EditResource<
        [Liftable(nameof(StatePremise), nameof(TState))] TSubState, 
        [Liftable(nameof(StateReportPremise), nameof(TStateReport))] TSubStateReport
        >(TResource resource, TSubState desiredState, TSubState maybeCurrentStateSnapshot)
        ;
}
