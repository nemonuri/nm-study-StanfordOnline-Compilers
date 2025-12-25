
using DscTool.Infrastructure;

namespace DscTool.Scoped.Graphs.Functors;

public readonly partial struct LowLevelPackedDigraphTupleFunctor<
    TSource, TSourceCondition, TSourceCategory, 
    THead, THeadCondition, THeadCategory,
    TNodeKey, TEdgeLabel> :
    ILowLevelPackedDigraphTupleFunctor<
        TSource, TSourceCondition, TSourceCategory, 
        THead, THeadCondition, THeadCategory,
        TNodeKey, TEdgeLabel>
    where TSourceCategory : IScopedCategory<TSource, TSourceCondition>
    where THeadCategory : IScopedCategory<THead, THeadCondition>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{

    private readonly 
    PackedDigraph<
        TNodeKey, TEdgeLabel, 
        LowLevelTupleFunctor<
        TSource, TSourceCondition, TSourceCategory, 
        THead, THeadCondition, THeadCategory>>
        _tupleFunctorGraph;

    private readonly NestedTargetCategory _targetCategory;
    private readonly NestedConditionLifter _conditionLifter;
    private readonly NestedLifter _lifter;
    private readonly NestedEmbedder _embedder;

    public LowLevelPackedDigraphTupleFunctor
    (
        PackedDigraph<TNodeKey, TEdgeLabel, TSourceCategory> sourceCategoryGraph,
        THeadCategory headCategory,
        TSourceCondition weakestSourceCondition,
        THeadCondition weakestHeadCondition,
        StrongestConditionLowLevelFactory<TSource, TSourceCondition> strongestSourceConditionFactory,
        THead defaultHead,
        THeadCondition defaultHeadCondition
    )
    {
        LowLevelTupleFunctor<
            TSource, TSourceCondition, TSourceCategory, 
            THead, THeadCondition, THeadCategory>
        SelectorImpl (TSourceCategory sourceCategory)
        {
            return new
            (
                sourceCategory,
                headCategory,
                weakestSourceCondition,
                weakestHeadCondition,
                strongestSourceConditionFactory,
                defaultHead,
                defaultHeadCondition
            );
        }

        _tupleFunctorGraph = sourceCategoryGraph.SelectValue(SelectorImpl);
        _targetCategory = new(_tupleFunctorGraph);
        _conditionLifter = new(_tupleFunctorGraph);
        _lifter = new(_tupleFunctorGraph);
        _embedder = new(_tupleFunctorGraph);
    }    

    [UnscopedRef] public ref readonly NestedTargetCategory TargetCategory => ref _targetCategory;

    [UnscopedRef] public ref readonly NestedConditionLifter ConditionLifter => ref _conditionLifter;

    [UnscopedRef] public ref readonly NestedLifter Lifter => ref _lifter;

    [UnscopedRef] public ref readonly NestedEmbedder Embedder => ref _embedder;
    

    public bool Satisfies
    (
        scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TSourceCondition> value, 
        scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)> condition
    )
    {
        PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)> liftedValue = default;
        PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)> liftedValuePostCondition = default;
        if (!_conditionLifter.TryMorph(in value, ref liftedValue, ref liftedValuePostCondition)) {return false;}
        return IsSufficient(in liftedValue, in condition);
    }

    public bool IsSufficient
    (
        scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)> sufficient, 
        scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)> necessary
    )
    {
        return _targetCategory.IsSufficient(in sufficient, in necessary);
    }
}

