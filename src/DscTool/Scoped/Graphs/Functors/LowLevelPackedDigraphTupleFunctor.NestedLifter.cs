
using DscTool.Infrastructure;

namespace DscTool.Scoped.Graphs.Functors;

public readonly partial struct LowLevelPackedDigraphTupleFunctor<
    TSource, TSourceCondition, TSourceCategory, 
    THead, THeadCondition, THeadCategory,
    TNodeKey, TEdgeLabel> where TSourceCategory : IScopedCategory<TSource, TSourceCondition>
    where THeadCategory : IScopedCategory<THead, THeadCondition>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    public readonly struct NestedLifter :
        IScopedHoareTripleMorphism<
            PackedDigraph<TNodeKey, TEdgeLabel, TSource>,
            PackedDigraph<TNodeKey, TEdgeLabel, TSourceCondition>,
            PackedDigraph<TNodeKey, TEdgeLabel, (THead, TSource)>,
            PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)>>
    {
        private readonly PackedDigraph<TNodeKey, TEdgeLabel, LowLevelTupleFunctor<TSource, TSourceCondition, TSourceCategory, THead, THeadCondition, THeadCategory>.NestedLifter> _lifter;
        private readonly PackedDigraph<TNodeKey, TEdgeLabel, TSourceCondition> _preCondition;

        public NestedLifter
        (
            PackedDigraph<
                TNodeKey, TEdgeLabel, 
                LowLevelTupleFunctor<
                TSource, TSourceCondition, TSourceCategory, 
                THead, THeadCondition, THeadCategory>>
            tupleFunctorGraph
        )
        {
            _lifter = tupleFunctorGraph.SelectValue(static a => a.Lifter);
            _preCondition = _lifter.SelectValue(static a => a.PreCondition);
        }

        public bool TryMorph
        (
            scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TSource> source, 
            [NotNullWhen(true)] scoped ref PackedDigraph<TNodeKey, TEdgeLabel, (THead, TSource)> result, 
            [NotNullWhen(true)] scoped ref PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)> postCondition
        )
        {
            var lifter = _lifter;

            (THead, TSource) resultFallback = default;
            (THeadCondition, TSourceCondition) postConditionFallback = default;
            if (!lifter.FallbackValue.TryMorph(in source.PackedMap.Fallback.NodeValue, ref resultFallback, ref postConditionFallback)) {return false;};

            ((THead, TSource) Result, (THeadCondition, TSourceCondition) PostCondition)?
            SelectorImpl(RawKeyValuePair<TNodeKey, TSource> pair)
            {
                (THead, TSource) result0 = default;
                (THeadCondition, TSourceCondition) postCondition0 = default;
                if (!lifter.GetEntryOrFallback(pair.Key).Value.NodeValue.TryMorph(in pair.Value, ref result0, ref postCondition0)) {return null;}

                return (result0, postCondition0);
            }

            var resultCandidate = source.SelectKeyValue(selector: SelectorImpl, fallback: (resultFallback, postConditionFallback));
            foreach (ref readonly var entry in resultCandidate.AsSpan)
            {
                if (!entry.Value.NodeValue.HasValue) {return false;}
            }

            result = resultCandidate.SelectValue(static a => a!.Value.Result);
            postCondition = resultCandidate.SelectValue(static a => a!.Value.PostCondition);
            return true;
        }

        [UnscopedRef] public ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TSourceCondition> PreCondition => ref _preCondition;
    }
}
