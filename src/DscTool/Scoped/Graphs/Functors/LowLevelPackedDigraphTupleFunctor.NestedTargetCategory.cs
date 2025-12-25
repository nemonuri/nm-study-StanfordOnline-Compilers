
using DscTool.Infrastructure;

namespace DscTool.Scoped.Graphs.Functors;

public readonly partial struct LowLevelPackedDigraphTupleFunctor<
    TSource, TSourceCondition, TSourceCategory, 
    THead, THeadCondition, THeadCategory,
    TNodeKey, TEdgeLabel> 
    where TSourceCategory : IScopedCategory<TSource, TSourceCondition>
    where THeadCategory : IScopedCategory<THead, THeadCondition>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    public readonly struct NestedTargetCategory :
        IScopedCategory<
            PackedDigraph<TNodeKey, TEdgeLabel, (THead, TSource)>, 
            PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)>>
    {

        private readonly ScopedPackedGraphCategory<
            (THead, TSource), (THeadCondition, TSourceCondition), 
            LowLevelTupleFunctor<
            TSource, TSourceCondition, TSourceCategory, 
            THead, THeadCondition, THeadCategory>.NestedTargetCategory, 
            TNodeKey, TEdgeLabel> _internalCategory;

        public NestedTargetCategory
        (
            PackedDigraph<
                TNodeKey, TEdgeLabel, 
                LowLevelTupleFunctor<
                TSource, TSourceCondition, TSourceCategory, 
                THead, THeadCondition, THeadCategory>>
            tupleFunctorGraph
        )
        {
            _internalCategory = new(tupleFunctorGraph.SelectValue(static s => s.TargetCategory));
        }

        public bool Equals(PackedDigraph<TNodeKey, TEdgeLabel, (THead, TSource)> x, PackedDigraph<TNodeKey, TEdgeLabel, (THead, TSource)> y) => _internalCategory.Equals(x, y);

        public int GetHashCode(PackedDigraph<TNodeKey, TEdgeLabel, (THead, TSource)> x) => _internalCategory.GetHashCode(x);

        public bool Satisfies(scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, (THead, TSource)> value, scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)> condition) =>
            _internalCategory.Satisfies(in value, in condition);

        public bool IsSufficient
        (
            scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)> sufficient, 
            scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)> necessary
        ) =>
            _internalCategory.IsSufficient(in sufficient, in necessary);
    }
}

