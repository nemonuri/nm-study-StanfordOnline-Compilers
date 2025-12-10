namespace DscTool;

using static MemberPathResolveStrategy;

public static class DesiredResourceStateTheory
{
    extension
    <TResource, TResourcePremise, TState, TStatePremise, TDiagnostic, TStateReport, TStateReportPremise, TTestReport,
     TDrsPremise>
    (scoped ref readonly 
        ReadOnlyTypeBox<
            (TResource, TResourcePremise, TState, TStatePremise, TDiagnostic, TStateReport, TStateReportPremise, TTestReport), 
             TDrsPremise> box)
        where TResourcePremise : IUnionPremise<TResource>, ITreePremise<TResource>
        where TState : IState<TResource>
        where TStatePremise : IUnionPremise<TState>, ITreePremise<TState>
        where TStateReport : IReport<TState, TDiagnostic>
        where TStateReportPremise : IUnionPremise<TStateReport>
        where TTestReport : IReport<bool, TDiagnostic>
        where TDrsPremise : IDesiredResourceStatePremise<TResource, TResourcePremise, TState, TStatePremise, TDiagnostic, TStateReport, TStateReportPremise, TTestReport>
    {
        public unsafe ref readonly 
        ReadOnlyTypeBox<
            (TResource, TResourcePremise, TState, TStatePremise, TDiagnostic, TStateReport, TStateReportPremise, TTestReport,
             TSubState, TSubStateReport), 
             TDrsPremise> Refine<TSubState, TSubStateReport>
             (TypeHint<(TSubState, TSubStateReport)> hint)
        {
#pragma warning disable CS9090
            return ref TypeBox.ReadOnlyBox<
            (TResource, TResourcePremise, TState, TStatePremise, TDiagnostic, TStateReport, TStateReportPremise, TTestReport,
             TSubState, TSubStateReport), 
             TDrsPremise>(in box.Self);
#pragma warning restore CS9090
        }

        
    }

    extension
    <TResource, TResourcePremise, TState, TStatePremise, TDiagnostic, TStateReport, TStateReportPremise, TTestReport,
     TDrsPremise, 
     [Liftable($"{nameof(box.Self)}.{nameof(box.Self.StatePremise)}", nameof(TState), 
        MemberPathResolveStrategy=ExtensionThis)] TSubState, 
     [Liftable($"{nameof(box.Self)}.{nameof(box.Self.StateReportPremise)}", nameof(TStateReport), 
        MemberPathResolveStrategy=ExtensionThis)] TSubStateReport>
    (scoped ref readonly 
        ReadOnlyTypeBox<
            (TResource, TResourcePremise, TState, TStatePremise, TDiagnostic, TStateReport, TStateReportPremise, TTestReport,
             TSubState, TSubStateReport), 
             TDrsPremise> box)
        where TResourcePremise : IUnionPremise<TResource>, ITreePremise<TResource>
        where TState : IState<TResource>
        where TStatePremise : IUnionPremise<TState>, ITreePremise<TState>
        where TStateReport : IReport<TState, TDiagnostic>
        where TStateReportPremise : IUnionPremise<TStateReport>
        where TTestReport : IReport<bool, TDiagnostic>
        where TDrsPremise : IDesiredResourceStatePremise<TResource, TResourcePremise, TState, TStatePremise, TDiagnostic, TStateReport, TStateReportPremise, TTestReport>
    {
        public unsafe ref readonly 
        ReadOnlyTypeBox<
            (TResource, TResourcePremise, TState, TStatePremise, TDiagnostic, TStateReport, TStateReportPremise, TTestReport),
             TDrsPremise> Unrefine()
        {
#pragma warning disable CS9090
            return ref TypeBox.ReadOnlyBox<
            (TResource, TResourcePremise, TState, TStatePremise, TDiagnostic, TStateReport, TStateReportPremise, TTestReport),
             TDrsPremise>(in box.Self);
#pragma warning restore CS9090
        }

        public ref readonly TSubStateReport RequestDesiredState(scoped ref readonly TResource resource)
        {
            ref readonly var self = ref box.Self;
            ref readonly var resP = ref self.ResourcePremise;
            var decomposed = self.ResourcePremise.Decompose(in resource);
            if (decomposed.IsEmpty)
            {
                // 'resource' is lifted atomic object.
                
            }
        }
    }
}