
namespace DscTool.Scoped.Sequences;

public readonly struct ScopedMemoryFaithfulFunctor<T, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder> : 
    IScopedFaithfulFunctor<
        Memory<T>, Memory<TCondition>, T, TCondition, TCategory,
        TConditionLifter, TLifter, TEmbedder>
    where TCategory : IScopedCategory<T, TCondition>
    where TConditionLifter : IScopedHoareTripleMorphism<Memory<TCondition>, TCondition, TCondition, TCondition>
    where TLifter : IScopedHoareTripleMorphism<Memory<T>, Memory<TCondition>, T, TCondition>
    where TEmbedder : IScopedHoareTripleMorphism<T, TCondition, Memory<T>, Memory<TCondition>>
{
    private readonly TCategory _category;
    private readonly TConditionLifter _conditionLifter;
    private readonly TLifter _lifter;
    private readonly TEmbedder _embedder;

    public ScopedMemoryFaithfulFunctor(TCategory category, TConditionLifter conditionLifter, TLifter lifter, TEmbedder embedder)
    {
        _category = category;
        _conditionLifter = conditionLifter;
        _lifter = lifter;
        _embedder = embedder;
    }

    [UnscopedRef] public ref readonly TCategory TargetCategory => ref _category;

    [UnscopedRef] public ref readonly TConditionLifter ConditionLifter => ref _conditionLifter;

    [UnscopedRef] public ref readonly TLifter Lifter => ref _lifter;

    [UnscopedRef] public ref readonly TEmbedder Embedder => ref _embedder;

    public bool Satisfies(scoped ref readonly Memory<TCondition> value, scoped ref readonly TCondition condition)
    {
        scoped var values = value.Span;
        for (int i = 0; i < values.Length; i++)
        {
            if (!_category.IsSufficient(in values[i], in condition)) {return false;}
        }
        return true;
    }

    public bool IsSufficient(scoped ref readonly TCondition sufficient, scoped ref readonly TCondition necessary)
    {
        return _category.IsSufficient(in sufficient, in necessary);
    }
}

