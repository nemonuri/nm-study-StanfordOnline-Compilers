namespace DscTool.Scoped;

public interface IScopedCategory<T, TCondition> :
    IEqualityComparer<T>,
    IScopedConditionChecker<T, TCondition>,
    IScopedSufficientChecker<TCondition>
{
}

public interface IScopedCategoriedCommand<T, TCondition, TCategory> :
    IScopedHoareTripleCommand<T, TCondition>
    where TCategory : IScopedCategory<T, TCondition>
{
    [UnscopedRef] ref readonly TCategory Category {get;}
}

public interface IScopedCategoriedMorphism<
    TSource, TSourceCondition, TSourceCategory,
    TTarget, TTargetCondition, TTargetCategory> :
    IScopedHoareTripleMorphism<TSource, TSourceCondition, TTarget, TTargetCondition>
    where TSourceCategory : IScopedCategory<TSource, TSourceCondition>
    where TTargetCategory : IScopedCategory<TTarget, TTargetCondition>
{
    [UnscopedRef] ref readonly TSourceCategory SourceCategory {get;}
    [UnscopedRef] ref readonly TTargetCategory TargetCategory {get;}
}

public interface IScopedConditionCategory<TSourceCondition, TTarget, TTargetCondition, TTargetCategory> :
    IScopedConditionChecker<TSourceCondition, TTargetCondition>,
    IScopedSufficientChecker<TTargetCondition>
    where TTargetCategory : IScopedCategory<TTarget, TTargetCondition>
{
    [UnscopedRef] ref readonly TTargetCategory TargetCategory {get;}
}

public interface IScopedFaithfulFunctor
<TSource, TSourceCondition, TTarget, TTargetCondition, TTargetCategory, TConditionLifter, TLifter, TEmbedder> :
    IScopedConditionCategory<TSourceCondition, TTarget, TTargetCondition, TTargetCategory>
    where TTargetCategory : IScopedCategory<TTarget, TTargetCondition>
    where TConditionLifter : IScopedHoareTripleMorphism<TSourceCondition, TTargetCondition, TTargetCondition, TTargetCondition>
    where TLifter : IScopedHoareTripleMorphism<TSource, TSourceCondition, TTarget, TTargetCondition>
    where TEmbedder : IScopedHoareTripleMorphism<TTarget, TTargetCondition, TSource, TSourceCondition>
{
    [UnscopedRef] ref readonly TConditionLifter ConditionLifter {get;}
    [UnscopedRef] ref readonly TLifter Lifter {get;}
    [UnscopedRef] ref readonly TEmbedder Embedder {get;}
}