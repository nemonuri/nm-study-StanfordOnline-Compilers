using DscTool.Infrastructure;

namespace DscTool.Scoped.Graphs.Functors;

public interface ILowLevelPackedDigraphTupleFunctor<
    TSource, TSourceCondition, TSourceCategory, 
    THead, THeadCondition, THeadCategory,
    TNodeKey, TEdgeLabel> :
    IScopedFaithfulFunctor<
        PackedDigraph<TNodeKey, TEdgeLabel, TSource>,
        PackedDigraph<TNodeKey, TEdgeLabel, TSourceCondition>,
        PackedDigraph<TNodeKey, TEdgeLabel, (THead, TSource)>,
        PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)>,
        LowLevelPackedDigraphTupleFunctor<
            TSource, TSourceCondition, TSourceCategory, 
            THead, THeadCondition, THeadCategory,
            TNodeKey, TEdgeLabel>.NestedTargetCategory,
        LowLevelPackedDigraphTupleFunctor<
            TSource, TSourceCondition, TSourceCategory, 
            THead, THeadCondition, THeadCategory,
            TNodeKey, TEdgeLabel>.NestedConditionLifter,
        LowLevelPackedDigraphTupleFunctor<
            TSource, TSourceCondition, TSourceCategory, 
            THead, THeadCondition, THeadCategory,
            TNodeKey, TEdgeLabel>.NestedLifter,
        LowLevelPackedDigraphTupleFunctor<
            TSource, TSourceCondition, TSourceCategory, 
            THead, THeadCondition, THeadCategory,
            TNodeKey, TEdgeLabel>.NestedEmbedder>
    where TSourceCategory : IScopedCategory<TSource, TSourceCondition>
    where THeadCategory : IScopedCategory<THead, THeadCondition>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{}

