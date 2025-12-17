
namespace DscTool.Scoped.Sequences;

public readonly struct ScopedMemoryFaithfulFunctor<T, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder> : 
    IScopedMemoryFaithfulFunctor<T, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder>
    where TCategory : IScopedCategory<T, TCondition>
    where TConditionLifter : IScopedMemoryConditionLifter<TCondition>
    where TLifter : IScopedMemoryLifter<T, TCondition>
    where TEmbedder : IScopedMemoryEmbedder<T, TCondition>
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




public readonly struct ScopedMemoryFaithfulFunctor<TAtom, T, TAtomicCondition, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder, TAtomicConditionChecker> : 
    IScopedMemoryFaithfulFunctor<TAtom, T, TAtomicCondition, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder>
    where TCategory : IScopedCategory<T, TCondition>
    where TConditionLifter : IScopedMemoryConditionLifter<TAtomicCondition, TCondition>
    where TLifter : IScopedMemoryLifter<TAtom, T, TAtomicCondition, TCondition>
    where TEmbedder : IScopedMemoryEmbedder<TAtom, T, TAtomicCondition, TCondition>
    where TAtomicConditionChecker : IScopedConditionChecker<TAtomicCondition, TCondition>
{
    private readonly TCategory _category;
    private readonly TConditionLifter _conditionLifter;
    private readonly TLifter _lifter;
    private readonly TEmbedder _embedder;
    private readonly TAtomicConditionChecker _atomicConditionChecker;

    public ScopedMemoryFaithfulFunctor
    (
        TCategory category, 
        TConditionLifter conditionLifter, 
        TLifter lifter, 
        TEmbedder embedder,
        TAtomicConditionChecker atomicConditionChecker
    )
    {
        _category = category;
        _conditionLifter = conditionLifter;
        _lifter = lifter;
        _embedder = embedder;
        _atomicConditionChecker = atomicConditionChecker;
    }

    [UnscopedRef] public ref readonly TCategory TargetCategory => ref _category;

    [UnscopedRef] public ref readonly TConditionLifter ConditionLifter => ref _conditionLifter;

    [UnscopedRef] public ref readonly TLifter Lifter => ref _lifter;

    [UnscopedRef] public ref readonly TEmbedder Embedder => ref _embedder;
    

    public bool Satisfies(scoped ref readonly Memory<TAtomicCondition> value, scoped ref readonly TCondition condition)
    {
        scoped var values = value.Span;
        for (int i = 0; i < values.Length; i++)
        {
            if (!_atomicConditionChecker.Satisfies(in values[i], in condition)) {return false;}
        }
        return true;
    }

    public bool IsSufficient(scoped ref readonly TCondition sufficient, scoped ref readonly TCondition necessary)
    {
        return _category.IsSufficient(in sufficient, in necessary);
    }
}
