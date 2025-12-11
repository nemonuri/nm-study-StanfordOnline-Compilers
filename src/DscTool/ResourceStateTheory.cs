using System.Diagnostics;

namespace DscTool;

public static class DesiredResourceStateTheory
{
    extension
    <TResource, TResourcePremise, TState, TStatePremise, TDiagnostic, TStateReport, TStateReportPremise, TTestReport,
     TDrsPremise>
    (scoped ref readonly 
        ReadOnlyTypeBox<
            (TResource, TResourcePremise, TState, TStatePremise, TDiagnostic, TStateReport, TStateReportPremise, TTestReport), 
             TDrsPremise> box)
        where TResource : ILiftable<TResource, TResource>, ISubUnionPremise<TResource, TResourcePremise>
        where TResourcePremise : IUnionPremise<TResource>, ITreePremise<TResource>
        where TState : IState<TResource>, ILiftable<TState, TState>, ISubUnionPremise<TState, TStatePremise>
        where TStatePremise : IUnionPremise<TState>, ITreePremise<TState>
        where TStateReport : IReport<TState, TDiagnostic>, ILiftable<TStateReport, TStateReport>, ISubUnionPremise<TStateReport, TStateReportPremise>
        where TStateReportPremise : IUnionPremise<TStateReport>
        where TTestReport : IReport<bool, TDiagnostic>
        where TDrsPremise : IDesiredResourceStatePremise<TResource, TResourcePremise, TState, TStatePremise, TDiagnostic, TStateReport, TStateReportPremise, TTestReport>
    {
        public unsafe ref readonly 
        ReadOnlyTypeBox<
            (TResource, TResourcePremise, TState, TStatePremise, TDiagnostic, TStateReport, TStateReportPremise, TTestReport,
             TSubState, TSubStateReport, TSubResource), 
             TDrsPremise> Refine<TSubState, TSubStateReport, TSubResource>
             (TypeHint<(TSubState, TSubStateReport, TSubResource)> hint)
        where TSubState : ILiftable<TState, TSubState>, ISubUnionPremise<TState, TStatePremise>
        where TSubStateReport : ILiftable<TStateReport, TSubStateReport>, ISubUnionPremise<TStateReport, TStateReportPremise>
        where TSubResource : ILiftable<TResource, TSubResource>, ISubUnionPremise<TResource, TResourcePremise>
        {
#pragma warning disable CS9090
            return ref TypeBox.ReadOnlyBox<
            (TResource, TResourcePremise, TState, TStatePremise, TDiagnostic, TStateReport, TStateReportPremise, TTestReport,
             TSubState, TSubStateReport, TSubResource), 
             TDrsPremise>(in box.Self);
#pragma warning restore CS9090
        }
    }

    extension
    <TResource, TResourcePremise, TState, TStatePremise, TDiagnostic, TStateReport, TStateReportPremise, TTestReport,
     TDrsPremise, 
     TSubState, TSubStateReport, TSubResource>
    (scoped ref readonly 
        ReadOnlyTypeBox<
            (TResource, TResourcePremise, TState, TStatePremise, TDiagnostic, TStateReport, TStateReportPremise, TTestReport,
             TSubState, TSubStateReport, TSubResource), 
             TDrsPremise> box)
        where TResource : ILiftable<TResource, TResource>, ISubUnionPremise<TResource, TResourcePremise>
        where TResourcePremise : IUnionPremise<TResource>, ITreePremise<TResource>
        where TState : IState<TResource>, ILiftable<TState, TState>, ISubUnionPremise<TState, TStatePremise>
        where TStatePremise : IUnionPremise<TState>, ITreePremise<TState>
        where TStateReport : IReport<TState, TDiagnostic>, ILiftable<TStateReport, TStateReport>, ISubUnionPremise<TStateReport, TStateReportPremise>
        where TStateReportPremise : IUnionPremise<TStateReport>
        where TTestReport : IReport<bool, TDiagnostic>
        where TDrsPremise : IDesiredResourceStatePremise<TResource, TResourcePremise, TState, TStatePremise, TDiagnostic, TStateReport, TStateReportPremise, TTestReport>
        where TSubState : ILiftable<TState, TSubState>, ISubUnionPremise<TState, TStatePremise>
        where TSubStateReport : ILiftable<TStateReport, TSubStateReport>, ISubUnionPremise<TStateReport, TStateReportPremise>
        where TSubResource : ILiftable<TResource, TSubResource>, ISubUnionPremise<TResource, TResourcePremise>
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

        public ref readonly TStateReport RequestDesiredState(scoped ref readonly TResource resource)
        {
            ref readonly var self = ref box.Self;
            
            if (!self.ResourcePremise.TryEmbed<TSubResource>(in resource, out var subResource))
            {
                throw new NotImplementedException();
            }
            ref readonly var subReport = ref self.RequestDesiredState<TSubState, TSubStateReport, TSubResource>(in subResource);
            ref readonly TStateReport report;
            if (self.StateReportPremise.CanLift<TSubStateReport>())
            {
                report = ref self.StateReportPremise.Lift(in subReport);
            }
            else if (subReport.FallBackLifter is {} ensuredLifter)
            {
                report = ref ensuredLifter(in subReport);
            }
            else
            {
                throw new NotImplementedException();
            }
            

            

            var decomposed = self.ResourcePremise.Decompose(in liftedResource);
            if (decomposed.IsEmpty)
            {
                // 'liftedResource' is lifted atomic resource.
                if (!self.ResourcePremise.TryEmbed<TSubResource>(in liftedResource, out var embeddedResource))
                {
                    throw new NotImplementedException();
                }

                ref readonly TSubStateReport report = ref self.RequestDesiredState<TSubState, TSubStateReport, TSubResource>(in embeddedResource);

                if (self.StateReportPremise.CanLift<TSubStateReport>())
                {
                    return ref self.StateReportPremise.Lift(in report);
                }
                else if (report.FallBackLifter is {} ensuredLifter)
                {
                    return ref ensuredLifter(in report);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                // 'liftedResource' is not lifted atomic resource.

            }
        }
    }
}
