using DscTool.Infrastructure;

namespace DscTool.Scoped.Graphs;


public readonly struct ScopedPackedGraphFaithfulFunctor<T, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder, TNodeKey, TEdgeLabel> :
    IScopedPackedDigraphFaithfulFunctor<T, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder, TNodeKey, TEdgeLabel>
    where TCategory : IScopedCategory<T, TCondition>
    where TConditionLifter : IScopedPackedDigraphConditionLifter<TCondition, TNodeKey, TEdgeLabel>
    where TLifter : IScopedPackedDigraphLifter<T, TCondition, TNodeKey, TEdgeLabel>
    where TEmbedder : IScopedPackedDigraphEmbedder<T, TCondition, TNodeKey, TEdgeLabel>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    private readonly TConditionLifter _conditionLifter;
    private readonly TLifter _lifter;
    private readonly TEmbedder _embedder;
    private readonly TCategory _category;

    public ScopedPackedGraphFaithfulFunctor(TConditionLifter conditionLifter, TLifter lifter, TEmbedder embedder, TCategory category)
    {
        _conditionLifter = conditionLifter;
        _lifter = lifter;
        _embedder = embedder;
        _category = category;
    }

    [UnscopedRef] public ref readonly TConditionLifter ConditionLifter => ref _conditionLifter;

    [UnscopedRef] public ref readonly TLifter Lifter => ref _lifter;

    [UnscopedRef] public ref readonly TEmbedder Embedder => ref _embedder;

    [UnscopedRef] public ref readonly TCategory TargetCategory => ref _category;

    public bool Satisfies(scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TCondition> value, scoped ref readonly TCondition condition)
    {
        if (!_category.IsSufficient(in value.PackedMap.Fallback.NodeValue, in condition)) {return false;}
        foreach (ref readonly var entry in value.AsSpan)
        {
            if (!_category.IsSufficient(in entry.Value.NodeValue, in condition)) {return false;}
        }
        return true;
    }

    public bool IsSufficient(scoped ref readonly TCondition sufficient, scoped ref readonly TCondition necessary)
    {
        return _category.IsSufficient(in sufficient, in necessary);
    }
}



public readonly struct ScopedPackedGraphFaithfulFunctor<TAtom, T, TAtomicCondition, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder, TAtomicConditionChecker, TNodeKey, TEdgeLabel> :
    IScopedPackedDigraphFaithfulFunctor<TAtom, T, TAtomicCondition, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder, TNodeKey, TEdgeLabel>
    where TCategory : IScopedCategory<T, TCondition>
    where TConditionLifter : IScopedPackedDigraphConditionLifter<TAtomicCondition, TCondition, TNodeKey, TEdgeLabel>
    where TLifter : IScopedPackedDigraphLifter<TAtom, T, TAtomicCondition, TCondition, TNodeKey, TEdgeLabel>
    where TEmbedder : IScopedPackedDigraphEmbedder<TAtom, T, TAtomicCondition, TCondition, TNodeKey, TEdgeLabel>
    where TAtomicConditionChecker : IScopedConditionChecker<TAtomicCondition, TCondition>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    private readonly TConditionLifter _conditionLifter;
    private readonly TLifter _lifter;
    private readonly TEmbedder _embedder;
    private readonly TCategory _category;
    private readonly TAtomicConditionChecker _atomicConditionChecker;

    public ScopedPackedGraphFaithfulFunctor
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


    [UnscopedRef] public ref readonly TConditionLifter ConditionLifter => ref _conditionLifter;

    [UnscopedRef] public ref readonly TLifter Lifter => ref _lifter;

    [UnscopedRef] public ref readonly TEmbedder Embedder => ref _embedder;

    [UnscopedRef] public ref readonly TCategory TargetCategory => ref _category;


    public bool Satisfies(scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TAtomicCondition> value, scoped ref readonly TCondition condition)
    {
        if (!_atomicConditionChecker.Satisfies(in value.PackedMap.Fallback.NodeValue, in condition)) {return false;}
        foreach (ref readonly var entry in value.AsSpan)
        {
            if (!_atomicConditionChecker.Satisfies(in entry.Value.NodeValue, in condition)) {return false;}
        }
        return true;
    }

    public bool IsSufficient(scoped ref readonly TCondition sufficient, scoped ref readonly TCondition necessary)
    {
        return _category.IsSufficient(in sufficient, in necessary);
    }
}