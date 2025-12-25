
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
    public readonly struct NestedConditionLifter :
        IScopedHoareTripleMorphism<
            PackedDigraph<TNodeKey, TEdgeLabel, TSourceCondition>,
            PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)>,
            PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)>,
            PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)>>
    {
        private readonly PackedDigraph<TNodeKey, TEdgeLabel, LowLevelTupleFunctor<TSource, TSourceCondition, TSourceCategory, THead, THeadCondition, THeadCategory>.NestedConditionLifter> _conditionLifter;
        private readonly PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)> _precondition;

        public NestedConditionLifter
        (
            PackedDigraph<
                TNodeKey, TEdgeLabel, 
                LowLevelTupleFunctor<
                TSource, TSourceCondition, TSourceCategory, 
                THead, THeadCondition, THeadCategory>>
            tupleFunctorGraph
        )
        {
            _conditionLifter = tupleFunctorGraph.SelectValue(static a => a.ConditionLifter);
            _precondition = _conditionLifter.SelectValue(static a => a.PreCondition);
        }


        public bool TryMorph
        (
            scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TSourceCondition> source,
            [NotNullWhen(true)] scoped ref PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)> result,
            [NotNullWhen(true)] scoped ref PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)> postCondition
        )
        {
            var conditionLifter = _conditionLifter;
            
            (THeadCondition, TSourceCondition) resultFallback = default;
            (THeadCondition, TSourceCondition) postConditionFallback = default;
            if (!conditionLifter.FallbackValue.TryMorph(in source.PackedMap.Fallback.NodeValue, ref resultFallback, ref postConditionFallback)) {return false;}

            ((THeadCondition, TSourceCondition) Result, (THeadCondition, TSourceCondition) PostCondition)?
            SelectorImpl(RawKeyValuePair<TNodeKey, TSourceCondition> pair)
            {
                (THeadCondition, TSourceCondition) result0 = default;
                (THeadCondition, TSourceCondition) postCondition0 = default;
                if (!conditionLifter.GetEntryOrFallback(pair.Key).Value.NodeValue.TryMorph(in pair.Value, ref result0, ref postCondition0)) {return null;}

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

        [UnscopedRef] public ref readonly PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)> PreCondition => ref _precondition;
    }
}

