namespace DscTool;

public interface IMorphismPremise<TSoucre, TTarget, TTargetPredicate>
{
    bool CanMorph(scoped ref readonly TSoucre soucre, [NotNullWhen(true)] out TTargetPredicate expectedResult);

    ref readonly TTarget Morph(scoped ref readonly TSoucre soucre);
}

public interface IFunctorPremise<
TSource, TSourcePredicate, TSourceSetPremise,
TTarget, TTargetPredicate, TTargetSetPremise, TTargetSubsetPremise,
TMorphismPremise, TPredicateMorphismPremise>
    where TSourceSetPremise : IConditionChecker<TSource, TSourcePredicate>
    where TTargetSetPremise : IConditionChecker<TTarget, TTargetPredicate>
    where TMorphismPremise : IMorphismPremise<TSource, TTarget, TTargetPredicate>
    where TPredicateMorphismPremise : IMorphismPremise<TSourcePredicate, TTargetPredicate, TTargetPredicate>
{
    ref readonly TSourceSetPremise SourceSetPremise {get;}
    ref readonly TTargetSetPremise TargetSetPremise {get;}
    ref readonly TTargetSubsetPremise TargetSubsetPremise {get;}
    ref readonly TMorphismPremise MorphismPremise {get;}
    ref readonly TPredicateMorphismPremise PredicateMorphismPremise {get;}
}

public interface ILiftedCommand<T, TPredicate>
{
    bool CanEmbedInvokeLift(scoped ref readonly T liftedValue, [NotNullWhen(true)] out TPredicate expectedResult);

    ref readonly T EmbedInvokeLift(scoped ref readonly T liftedValue);
}

public interface ILiftedComposer<T, TPredicate>
{
    bool CanEmbedComposeLift(scoped ReadOnlySpan<T> liftedValues, [NotNullWhen(true)] out TPredicate expectedResult);

    ref readonly T EmbedComposeLift(scoped ReadOnlySpan<T> liftedValues);
}

public interface ILiftedDecomposer<T, TPredicate>
{
    bool CanEmbedDecomposeLift(scoped ReadOnlySpan<T> liftedValues, [NotNullWhen(true)] out TPredicate expectedResult);

    ref readonly T EmbedDecomposeLift(scoped ReadOnlySpan<T> liftedValues);
}


