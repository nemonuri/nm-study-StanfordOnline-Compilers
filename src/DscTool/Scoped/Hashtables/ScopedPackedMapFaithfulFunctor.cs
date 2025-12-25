using DscTool.Infrastructure;

namespace DscTool.Scoped.Hashtables;

public readonly struct ScopedPackedMapFaithfulFunctor<T, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder, TKey> :
    IScopedPackedMapFaithfulFunctor<T, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder, TKey>
    where TCategory : IScopedCategory<T, TCondition>
    where TConditionLifter : IScopedPackedMapConditionLifter<TCondition, TKey>
    where TLifter : IScopedPackedMapLifter<T, TCondition, TKey>
    where TEmbedder : IScopedPackedMapEmbedder<T, TCondition, TKey>
    where TKey : IEquatable<TKey>
{
    private readonly TConditionLifter _conditionLifter;
    private readonly TLifter _lifter;
    private readonly TEmbedder _embedder;
    private readonly TCategory _category;

    public ScopedPackedMapFaithfulFunctor(TConditionLifter conditionLifter, TLifter lifter, TEmbedder embedder, TCategory category)
    {
        _conditionLifter = conditionLifter;
        _lifter = lifter;
        _embedder = embedder;
        _category = category;
    }

    [UnscopedRef] public ref readonly TCategory TargetCategory => ref _category;

    [UnscopedRef] public ref readonly TConditionLifter ConditionLifter => ref _conditionLifter;

    [UnscopedRef] public ref readonly TLifter Lifter => ref _lifter;

    [UnscopedRef] public ref readonly TEmbedder Embedder => ref _embedder;

    public bool Satisfies
    (
        scoped ref readonly PackedMap<TKey, TCondition> value, 
        scoped ref readonly TCondition condition
    )
    {
        if (!_category.IsSufficient(in value.Fallback, in condition)) {return false;}
        foreach (ref readonly var entry in value.AsSpan)
        {
            if (!_category.IsSufficient(in entry.Value, in condition)) {return false;}
        }
        return true;
    }

    public bool IsSufficient(scoped ref readonly TCondition sufficient, scoped ref readonly TCondition necessary)
    {
        return _category.IsSufficient(in sufficient, in necessary);
    }
}






public readonly struct ScopedPackedMapFaithfulFunctor<TAtom, T, TAtomicCondition, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder, TAtomicConditionChecker, TKey> :
    IScopedPackedMapFaithfulFunctor<TAtom, T, TAtomicCondition, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder, TKey>
    where TCategory : IScopedCategory<T, TCondition>
    where TConditionLifter : IScopedPackedMapConditionLifter<TAtomicCondition, TCondition, TKey>
    where TLifter : IScopedPackedMapLifter<TAtom, T, TAtomicCondition, TCondition, TKey>
    where TEmbedder : IScopedPackedMapEmbedder<TAtom, T, TAtomicCondition, TCondition, TKey>
    where TAtomicConditionChecker : IScopedConditionChecker<TAtomicCondition, TCondition>
    where TKey : IEquatable<TKey>
{
    private readonly TCategory _category;
    private readonly TConditionLifter _conditionLifter;
    private readonly TLifter _lifter;
    private readonly TEmbedder _embedder;
    private readonly TAtomicConditionChecker _atomicConditionChecker;

    public ScopedPackedMapFaithfulFunctor
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

    public bool Satisfies(scoped ref readonly PackedMap<TKey, TAtomicCondition> value, scoped ref readonly TCondition condition)
    {
        if (!_atomicConditionChecker.Satisfies(in value.Fallback, in condition)) {return false;}
        foreach (ref readonly var entry in value.AsSpan)
        {
            if (!_atomicConditionChecker.Satisfies(in entry.Value, in condition)) {return false;}
        }
        return true;
    }

    public bool IsSufficient(scoped ref readonly TCondition sufficient, scoped ref readonly TCondition necessary)
    {
        return _category.IsSufficient(in sufficient, in necessary);
    }
}
