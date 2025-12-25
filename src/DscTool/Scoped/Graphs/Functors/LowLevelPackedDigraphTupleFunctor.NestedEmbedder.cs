
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
    public readonly struct NestedEmbedder :
        IScopedHoareTripleMorphism<
            PackedDigraph<TNodeKey, TEdgeLabel, (THead, TSource)>,
            PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)>,
            PackedDigraph<TNodeKey, TEdgeLabel, TSource>,
            PackedDigraph<TNodeKey, TEdgeLabel, TSourceCondition>>
    {
        private readonly PackedDigraph<TNodeKey, TEdgeLabel, LowLevelTupleFunctor<TSource, TSourceCondition, TSourceCategory, THead, THeadCondition, THeadCategory>.NestedEmbedder> _embedder;
        private readonly PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)> _preCondition;

        public NestedEmbedder
        (
            PackedDigraph<
                TNodeKey, TEdgeLabel, 
                LowLevelTupleFunctor<
                TSource, TSourceCondition, TSourceCategory, 
                THead, THeadCondition, THeadCategory>>
            tupleFunctorGraph
        )
        {
            _embedder = tupleFunctorGraph.SelectValue(static a => a.Embedder);
            _preCondition = _embedder.SelectValue(static a => a.PreCondition);
        }

        public bool TryMorph
        (
            scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, (THead, TSource)> source, 
            [NotNullWhen(true)] scoped ref PackedDigraph<TNodeKey, TEdgeLabel, TSource> result, 
            [NotNullWhen(true)] scoped ref PackedDigraph<TNodeKey, TEdgeLabel, TSourceCondition> postCondition
        )
        {
            var embedder = _embedder;

            TSource? resultFallback = default;
            TSourceCondition? postConditionFallback = default;
            if (!embedder.FallbackValue.TryMorph(in source.PackedMap.Fallback.NodeValue, ref resultFallback, ref postConditionFallback)) {return false;};

            (TSource Result, TSourceCondition PostCondition)?
            SelectorImpl(RawKeyValuePair<TNodeKey, (THead, TSource)> pair)
            {
                TSource? result0 = default;
                TSourceCondition? postCondition0 = default;
                if (!embedder.GetEntryOrFallback(pair.Key).Value.NodeValue.TryMorph(in pair.Value, ref result0, ref postCondition0)) {return null;}

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

        [UnscopedRef] public ref readonly PackedDigraph<TNodeKey, TEdgeLabel, (THeadCondition, TSourceCondition)> PreCondition => ref _preCondition;
    }
}

