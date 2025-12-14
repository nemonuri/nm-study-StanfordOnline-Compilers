namespace DscTool;

public static class FunctorTheory
{
    extension<
        TSource, TSourcePredicate, TSourceSetPremise,
        TTarget, TTargetPredicate, TTargetSetPremise, TTargetSubsetPremise,
        TMorphismPremise, TPredicateMorphismPremise,
        TFunctorPremise
    >(scoped ref readonly ReadOnlyTypeBox<(
        TSource, TSourcePredicate, TSourceSetPremise,
        TTarget, TTargetPredicate, TTargetSetPremise, TTargetSubsetPremise,
        TMorphismPremise, TPredicateMorphismPremise
        ), TFunctorPremise> box)
        where TSourceSetPremise : IConditionChecker<TSource, TSourcePredicate>
        where TTargetSetPremise : IConditionChecker<TTarget, TTargetPredicate>
        where TMorphismPremise : IMorphismPremise<TSource, TTarget, TTargetPredicate>
        where TPredicateMorphismPremise : IMorphismPremise<TSourcePredicate, TTargetPredicate, TTargetPredicate>
        where TFunctorPremise : IFunctorPremise<
            TSource, TSourcePredicate, TSourceSetPremise,
            TTarget, TTargetPredicate, TTargetSetPremise, TTargetSubsetPremise,
            TMorphismPremise, TPredicateMorphismPremise>
    {
        public static ref readonly ReadOnlyTypeBox<(
        TSource, TSourcePredicate, TSourceSetPremise,
        TTarget, TTargetPredicate, TTargetSetPremise, TTargetSubsetPremise,
        TMorphismPremise, TPredicateMorphismPremise
        ), TFunctorPremise>
        Box(ref readonly TFunctorPremise source) =>
            ref TypeBox.ReadOnlyBox<(
            TSource, TSourcePredicate, TSourceSetPremise,
            TTarget, TTargetPredicate, TTargetSetPremise, TTargetSubsetPremise,
            TMorphismPremise, TPredicateMorphismPremise
            ), TFunctorPremise>(in source);
        
        public bool CanMorphElement(scoped ref readonly TSource source, [NotNullWhen(true)] out TTargetPredicate? expectedResult) =>
            box.Self.MorphismPremise.CanMorph(in source, out expectedResult);
        
        //public ref readonly LiftCommand<Tc>
    }
}


