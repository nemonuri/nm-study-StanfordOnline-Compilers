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

public interface ISupportScopedConditionCategory<TSourceCondition, TTargetCondition, TConditionCategory>
    where TConditionCategory : IScopedConditionChecker<TSourceCondition, TTargetCondition>
{
    [UnscopedRef] ref readonly TConditionCategory ConditionCategory {get;}
}
