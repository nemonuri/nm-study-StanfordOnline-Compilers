namespace DscTool;

public interface IDesiredResourceStatePremise
    <TResource, TResourcePremise, TState, TStatePremise, TDiagnostic, TStateReport, TStateReportPremise, TTestReport> :
    IDiagnosticCondition<TDiagnostic>
    where TResource : ILiftable<TResource, TResource>, ISubUnionPremise<TResource, TResourcePremise>
    where TResourcePremise : IUnionPremise<TResource>, ITreePremise<TResource>
    where TState : IState<TResource>, ILiftable<TState, TState>, ISubUnionPremise<TState, TStatePremise>
    where TStatePremise : IUnionPremise<TState>, ITreePremise<TState>
    where TStateReport : IReport<TState, TDiagnostic>, ILiftable<TStateReport, TStateReport>, ISubUnionPremise<TStateReport, TStateReportPremise>
    where TStateReportPremise : IUnionPremise<TStateReport>
    where TTestReport : IReport<bool, TDiagnostic>
{
    ref readonly TResourcePremise ResourcePremise {get;}

    ref readonly TStatePremise StatePremise {get;}

    ref readonly TStateReportPremise StateReportPremise {get;}

    bool IsHandleable(scoped ref readonly TStateReport report);

    ref readonly TSubStateReport RequestDesiredState
        <TSubState, TSubStateReport, TSubResource>
        (scoped ref readonly TSubResource resource)
        where TSubState : ILiftable<TState, TSubState>, ISubUnionPremise<TState, TStatePremise>
        where TSubStateReport : ILiftable<TStateReport, TSubStateReport>, ISubUnionPremise<TStateReport, TStateReportPremise>
        where TSubResource : ILiftable<TResource, TSubResource>, ISubUnionPremise<TResource, TResourcePremise>
        ;

    ref readonly TSubStateReport RequestStateSnapshot
        <TSubState, TSubStateReport, TSubResource>
        (scoped ref readonly TSubResource resource)
        where TSubState : ILiftable<TState, TSubState>, ISubUnionPremise<TState, TStatePremise>
        where TSubStateReport : ILiftable<TStateReport, TSubStateReport>, ISubUnionPremise<TStateReport, TStateReportPremise>
        where TSubResource : ILiftable<TResource, TSubResource>, ISubUnionPremise<TResource, TResourcePremise>
        ;

    ref readonly TTestReport TestState
        <TSubState>
        (scoped ref readonly TSubState desiredState, scoped ref readonly TSubState stateSnapshot)
        where TSubState : ILiftable<TState, TSubState>, ISubUnionPremise<TState, TStatePremise>
        ;

    ref readonly TSubStateReport EditResource
        <TSubState, TSubStateReport, TSubResource>
        (scoped ref readonly TSubResource resource, scoped ref readonly TSubState desiredState, scoped ref readonly TSubState maybeCurrentStateSnapshot)
        where TSubState : ILiftable<TState, TSubState>, ISubUnionPremise<TState, TStatePremise>
        where TSubStateReport : ILiftable<TStateReport, TSubStateReport>, ISubUnionPremise<TStateReport, TStateReportPremise>
        where TSubResource : ILiftable<TResource, TSubResource>, ISubUnionPremise<TResource, TResourcePremise>
        ;
}