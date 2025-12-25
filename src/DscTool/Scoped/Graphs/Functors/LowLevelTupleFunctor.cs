
namespace DscTool.Scoped.Graphs.Functors;

public readonly partial struct LowLevelTupleFunctor<
    TSource, TSourceCondition, TSourceCategory, 
    THead, THeadCondition, THeadCategory> :
    ILowLevelTupleFunctor<
    TSource, TSourceCondition, TSourceCategory, 
    THead, THeadCondition, THeadCategory>
    where TSourceCategory : IScopedCategory<TSource, TSourceCondition>
    where THeadCategory : IScopedCategory<THead, THeadCondition>
{
    private readonly NestedTargetCategory _targetCategory;
    private readonly NestedConditionLifter _conditionLifter;
    private readonly NestedLifter _lifter;
    private readonly NestedEmbedder _embedder;

    public LowLevelTupleFunctor
    (
        TSourceCategory sourceCategory,
        THeadCategory headCategory,
        TSourceCondition weakestSourceCondition,
        THeadCondition weakestHeadCondition,
        StrongestConditionLowLevelFactory<TSource, TSourceCondition> strongestSourceConditionFactory,
        THead defaultHead,
        THeadCondition defaultHeadCondition
    )
    {
        Guard.IsNotNull(strongestSourceConditionFactory);

        _targetCategory = new(sourceCategory, headCategory);
        _conditionLifter = new(weakestSourceCondition, weakestHeadCondition);
        _lifter = new(weakestSourceCondition, strongestSourceConditionFactory, defaultHead, defaultHeadCondition);
        _embedder = new(weakestSourceCondition, weakestHeadCondition, strongestSourceConditionFactory);
    }

    public bool Satisfies(scoped ref readonly TSourceCondition value, scoped ref readonly (THeadCondition, TSourceCondition) condition)
    {
        return _targetCategory._sourceCategory.IsSufficient(in value, in condition.Item2);
    }

    public bool IsSufficient(scoped ref readonly (THeadCondition, TSourceCondition) sufficient, scoped ref readonly (THeadCondition, TSourceCondition) necessary)
    {
        return _targetCategory.IsSufficient(in sufficient, in necessary);
    }

    [UnscopedRef] public ref readonly NestedConditionLifter ConditionLifter => ref _conditionLifter;

    [UnscopedRef] public ref readonly NestedLifter Lifter => ref _lifter;

    [UnscopedRef] public ref readonly NestedEmbedder Embedder => ref _embedder;

    [UnscopedRef] public ref readonly NestedTargetCategory TargetCategory => ref _targetCategory;
}

