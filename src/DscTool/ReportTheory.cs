namespace DscTool;

internal static class ReportTheory
{
    extension<TDesired, TDiagnostic, TReport, TDiagnosticCondition>(scoped ref readonly ReadOnlyTypeBox<(TDesired, TDiagnostic, TDiagnosticCondition), TReport> box)
        where TReport : IReport<TDesired, TDiagnostic>
        where TDiagnosticCondition : IDiagnosticCondition<TDiagnostic>
    {
        public TOut Match<TOut>
        (
            Func<TDesired, TOut> caseOfSussess,
            ReadOnlySpanFunc<TDiagnostic, TOut> caseOfFail,
            scoped ref readonly TDiagnosticCondition diagnosticCondition,
            bool ensureEmptyLikeDiagnostics = true
        )
        {
            Guard.IsNotNull(caseOfSussess);
            Guard.IsNotNull(caseOfFail);

            ref readonly var self = ref box.Self;

            if 
            (
                self.TryGetDesired(out var desired) &&
                (!ensureEmptyLikeDiagnostics || self.Diagnostics.IsEmptyLike(diagnosticCondition))
            )
            {
                return caseOfSussess(desired);
            }
            else
            {
                return caseOfFail(self.Diagnostics);
            }
        }
        
        public bool IsSuccess(scoped ref readonly TDiagnosticCondition diagnosticCondition) => 
            box.Match(static _ => true, static _ => false, in diagnosticCondition, true);
    }

    extension<TDesired, TDiagnostic, TReport>(scoped ref readonly ReadOnlyTypeBox<(TDesired, TDiagnostic), TReport> box)
        where TReport : IReport<TDesired, TDiagnostic>
    {
        public TDesired? GetDesiredOrDefault() => box.Self.TryGetDesired(out var desired) ? desired : default;

        public bool IsSuccess(Func<TDiagnostic, bool>? diagnosticCondition = null)
        {
            AdHocDiagnosticCondition<TDiagnostic> dc = new(diagnosticCondition);
            return TypeBox.ReadOnlyBox<(TDesired, TDiagnostic, AdHocDiagnosticCondition<TDiagnostic>), TReport>(in box.Self)
                .Match(static _ => true, static _ => false, in dc, true);
        }


        public TReport Compose(scoped ref readonly TReport other, ReportFactory<TDesired, TDiagnostic, TReport> factory)
        {
            Guard.IsNotNull(box.Self); Guard.IsNotNull(other); Guard.IsNotNull(factory);
            
            ReadOnlySpan<TDiagnostic> newDiagnostics = [..box.Self.Diagnostics, ..other.Diagnostics];
            return factory(
                box.GetDesiredOrDefault(), 
                TypeBox.ReadOnlyBox<(TDesired, TDiagnostic), TReport>(in other).GetDesiredOrDefault(),
                newDiagnostics
            );
        }

        public TReport Compose(ReadOnlySpan<TReport> others, ReportFactory<TDesired, TDiagnostic, TReport> factory)
        {
            TReport acc = box.Self;
            foreach (var other in others)
            {
                acc = TypeBox.ReadOnlyBox<(TDesired, TDiagnostic), TReport>(in acc).Compose(in other, factory);
            }
            return acc;
        }
    }
}

file static class ReadOnlySpanExtensions
{
    extension<TDiagnostic, TDiagnosticCondition>(ReadOnlySpan<TDiagnostic> span)
        where TDiagnosticCondition : IDiagnosticCondition<TDiagnostic>
    {
        public bool IsEmptyLike(TDiagnosticCondition dc)
        {
            foreach (var item in span)
            {
                if (dc.CheckDiagnostic(item)) {return false;}
            }
            return true;
        }
    }
}
