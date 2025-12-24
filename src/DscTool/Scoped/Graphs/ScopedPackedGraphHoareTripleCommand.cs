using DscTool.Infrastructure;
using Okt = DscTool.Infrastructure.OptionalKeyTagger;
using Vct = DscTool.Infrastructure.ValueChoiceTagger;

namespace DscTool.Scoped.Graphs;

public readonly struct ScopedPackedGraphHoareTripleCommand<T, TCondition, TCommand, TNodeKey, TEdgeLabel> :
    IScopedHoareTripleCommand<
        PackedDigraph<TNodeKey, TEdgeLabel, T>,
        PackedDigraph<TNodeKey, TEdgeLabel, TCondition>>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
    where TCommand : IScopedHoareTripleCommand<T, TCondition>
{
    private readonly PackedDigraph<TNodeKey, TEdgeLabel, TCommand> _commandGraph;
    private readonly PackedDigraph<TNodeKey, TEdgeLabel, TCondition> _precondition;

    [UnscopedRef] public ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TCondition> PreCondition => ref _precondition;

    public ScopedPackedGraphHoareTripleCommand(PackedDigraph<TNodeKey, TEdgeLabel, TCommand> commandGraph)
    {
        _commandGraph = commandGraph;
        _precondition = commandGraph.SelectValue(static v => v.PreCondition);
    }

    private struct NodeKeyState
    {
        public bool IsCommonKey;
        public bool Invoked;
        public bool IsSuccessed;
        public T? Result;
        public TCondition? PostCondition;

        public NodeKeyState()
        {
            IsCommonKey = false;
            Invoked = false;
            IsSuccessed = false;
            Result = default;
            PostCondition = default;
        }
    }

    public bool TryInvoke
    (
        scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, T> source, 
        [NotNullWhen(true)] scoped ref PackedDigraph<TNodeKey, TEdgeLabel, T> result, 
        [NotNullWhen(true)] scoped ref PackedDigraph<TNodeKey, TEdgeLabel, TCondition> postCondition
    )
    {
        PackedMap<TNodeKey, NodeKeyState> statesMap = source.PackedMap.SelectValue(static _ => new NodeKeyState());

        //--- First loop : Check common keys first ---
        foreach (ref RawKeyValuePair<TNodeKey, NodeKeyState> entry in statesMap.AsSpan)
        {
            ref NodeKeyState curState = ref entry.Value;

            //--- Check if current node-key is already checked ---
            if (curState.Invoked)
            {
                if (!curState.IsSuccessed) {return false;}
            }
            //---|

            //--- Check if current node-key is common key ---
            curState.IsCommonKey = _commandGraph.PackedMap.ContainsKey(entry.Key);

            // if not, continue
            if (!curState.IsCommonKey) {continue;}
            //---|

            if (!DepthFirstCheck(in source, in _commandGraph, in statesMap, Vct.Choice1(entry.Key))) {return false;}
        }
        //---|

        //--- Second loop : Check unchecked node-keys ---
        foreach (ref RawKeyValuePair<TNodeKey, NodeKeyState> entry in statesMap.AsSpan)
        {
            ref NodeKeyState curState = ref entry.Value;

            if (curState.Invoked) {continue;}

            if (!DepthFirstCheck(in source, in _commandGraph, in statesMap, Vct.Choice2((entry.Key, (OptionalKey<TNodeKey>)Okt.None)))) {return false;}
        }
        //---|

        T ResultSelectorImpl(TNodeKey nodeKey) => statesMap.GetEntryOrFallback(nodeKey).Value.Result!;
        TCondition PostConditionSelectorImpl(TNodeKey nodeKey) => statesMap.GetEntryOrFallback(nodeKey).Value.PostCondition!;

        result = source.SelectKey(ResultSelectorImpl, source.FallbackValue);
        postCondition = source.SelectKey(PostConditionSelectorImpl, _commandGraph.FallbackValue.PreCondition);
        return true;

        static bool DepthFirstCheck
        (
            scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, T> sourceGraph,
            scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TCommand> commandGraph,
            scoped ref readonly PackedMap<TNodeKey, NodeKeyState> statesMap, 
            ValueChoice<TNodeKey, (TNodeKey SourceKey, OptionalKey<TNodeKey> CommandKey)> commonOrDiffrentKey
        )
        {
            (TNodeKey sourceKey, OptionalKey<TNodeKey> commandKey) = 
                commonOrDiffrentKey.Match<(TNodeKey, OptionalKey<TNodeKey>)>
                (
                    caseOfChoice1: static vk => (vk, Okt.Some(vk)),
                    caseOfChoice2: static pair => pair
                );

            ref NodeKeyState state = ref statesMap.GetValueRef(sourceKey);
            if (state.Invoked) {return state.IsSuccessed;}

            //--- Get pairs of value, conditon, category ---
            NodeValueAdjacentsPair<TNodeKey, TEdgeLabel, T> sourcePair = sourceGraph.GetEntryOrFallback(sourceKey).Value;
            NodeValueAdjacentsPair<TNodeKey, TEdgeLabel, TCommand> commandPair = commandGraph.GetEntryOrFallback(commandKey).Value;
            //---|

            //--- Check value staisfies condition ---
            state.IsSuccessed = commandPair.NodeValue.TryInvoke(in sourcePair.NodeValue, ref state.Result, ref state.PostCondition);

            // If result is false, return false.
            if (state.IsSuccessed == false) {return false;}
            //---|

            //--- Visit adjacents ---
            PackedMap<TEdgeLabel, OptionalKey<TNodeKey>> commandAdjacentsPackedMap = commandGraph.GetAdjacentsAsPackedMap(commandKey);
            foreach (ref readonly LabeledNode<TNodeKey, TEdgeLabel> adj in sourcePair.Adjacents.Span)
            {
                TNodeKey nextValueKey = adj.Node;
                OptionalKey<TNodeKey> nextConditionKey = commandAdjacentsPackedMap.GetEntryOrFallback(adj.Label).Value;

                // If result is false, return false.
                if (!DepthFirstCheck(in sourceGraph, in commandGraph, in statesMap, Vct.Choice2((nextValueKey, nextConditionKey)))) {return false;}
            }
            //---|

            return true;
        }
    }
}
