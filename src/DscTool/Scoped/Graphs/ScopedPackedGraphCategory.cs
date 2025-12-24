using DscTool.Infrastructure;
using DscTool.Scoped.Hashtables;
using static DscTool.Infrastructure.Extensions.PackedMap;
using Okt = DscTool.Infrastructure.OptionalKeyTagger;
using Vct = DscTool.Infrastructure.ValueChoiceTagger;

namespace DscTool.Scoped.Graphs;

public readonly struct ScopedPackedGraphCategory<T, TCondition, TCategory, TNodeKey, TEdgeLabel> :
    IScopedCategory<PackedDigraph<TNodeKey, TEdgeLabel, T>, PackedDigraph<TNodeKey, TEdgeLabel, TCondition>>
    where TCategory : IScopedCategory<T, TCondition>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    private readonly PackedDigraph<TNodeKey, TEdgeLabel, TCategory> _categoryGraph;
    private readonly ScopedPackedMapCategory<T, TCondition, TCategory, TNodeKey> _nodeKeyValueCategoryMap;

    public ScopedPackedGraphCategory(PackedDigraph<TNodeKey, TEdgeLabel, TCategory> categoryGraph)
    {
        _categoryGraph = categoryGraph;
        _nodeKeyValueCategoryMap = new(categoryGraph.GetNodeValuesAsPackedMap());
    }

    public bool Equals(PackedDigraph<TNodeKey, TEdgeLabel, T> xGraph, PackedDigraph<TNodeKey, TEdgeLabel, T> yGraph)
    {
        //--- Check NodeKey-NodeValue maps are equal ---
        if (!_nodeKeyValueCategoryMap.Equals(xGraph.GetNodeValuesAsPackedMap(), yGraph.GetNodeValuesAsPackedMap())) {return false;}
        //---|

        //--- Check Adjacents are equal ---
        foreach (ref readonly var entry in xGraph.AsSpan)
        {
            TNodeKey curNodeKey = entry.Key;
            var xMap = xGraph.GetAdjacentsAsPackedMap(curNodeKey);
            var yMap = yGraph.GetAdjacentsAsPackedMap(curNodeKey);
            if (!xMap.ExtensionallyEquals(in yMap)) {return false;}
        }
        //---|

        return true;
    }

    public int GetHashCode(PackedDigraph<TNodeKey, TEdgeLabel, T> xGraph)
    {
        int hashCode = _nodeKeyValueCategoryMap.GetHashCode(xGraph.GetNodeValuesAsPackedMap());

        foreach (ref readonly var entry in xGraph.AsSpan)
        {
            var xMap = xGraph.GetAdjacentsAsPackedMap(entry.Key);
            hashCode ^= xMap.GetExtensionalHashCode();
        }

        return hashCode;
    }

    internal struct NodeKeyState
    {
        public bool IsCommonKey;
        public DfcState DfcState;

        public NodeKeyState()
        {
            IsCommonKey = false;
            DfcState = new();
        }
    }

    public bool Satisfies(scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, T> value, scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TCondition> condition)
    {
        PackedMap<TNodeKey, NodeKeyState> commonKeysMap = value.PackedMap.SelectValue(static _ => new NodeKeyState());

        //--- First loop : Check common keys first ---
        foreach (ref RawKeyValuePair<TNodeKey, NodeKeyState> entry in commonKeysMap.AsSpan)
        {
            ref NodeKeyState curState = ref entry.Value;

            //--- Check if current node-key is already checked ---
            if (curState.DfcState.Checked)
            {
                if (!curState.DfcState.CheckResult) {return false;}
            }
            //---|

            //--- Check if current node-key is common key ---
            curState.IsCommonKey = condition.PackedMap.ContainsKey(entry.Key) && _categoryGraph.PackedMap.ContainsKey(entry.Key);

            // if not, continue
            if (!curState.IsCommonKey) {continue;}
            //---|

            if (!DepthFirstCheck(in value, in condition, in _categoryGraph, in commonKeysMap, Vct.Choice1(entry.Key))) {return false;}
        }
        //---|

        //--- Second loop : Check unchecked node-keys ---
        foreach (ref RawKeyValuePair<TNodeKey, NodeKeyState> entry in commonKeysMap.AsSpan)
        {
            ref NodeKeyState curState = ref entry.Value;

            if (curState.DfcState.Checked) {continue;}

            if (!DepthFirstCheck(in value, in condition, in _categoryGraph, in commonKeysMap, Vct.Choice2((entry.Key, (OptionalKey<TNodeKey>)Okt.None, (OptionalKey<TNodeKey>)Okt.None)))) {return false;}
        }
        //---|

        return true;

        static bool DepthFirstCheck
        (
            scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, T> valueGraph,
            scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TCondition> conditionGraph,
            scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TCategory> categoryGraph,
            scoped ref readonly PackedMap<TNodeKey, NodeKeyState> statesMap, 
            ValueChoice<TNodeKey, (TNodeKey ValueKey, OptionalKey<TNodeKey> ConditonKey, OptionalKey<TNodeKey> CategoryKey)> commonOrDiffrentKey
        )
        {
            (TNodeKey valueKey, OptionalKey<TNodeKey> conditionKey, OptionalKey<TNodeKey> categoryKey) = 
                commonOrDiffrentKey.Match<(TNodeKey, OptionalKey<TNodeKey>, OptionalKey<TNodeKey>)>
                (
                    caseOfChoice1: static vk => (vk, Okt.Some(vk), Okt.Some(vk)),
                    caseOfChoice2: static pair => pair
                );

            ref DfcState state = ref statesMap.GetValueRef(valueKey).DfcState;
            if (state.Checked) {return state.CheckResult;}

            //--- Get pairs of value, conditon, category ---
            NodeValueAdjacentsPair<TNodeKey, TEdgeLabel, T> valuePair = valueGraph.GetEntryOrFallback(valueKey).Value;
            NodeValueAdjacentsPair<TNodeKey, TEdgeLabel, TCondition> conditionPair = conditionGraph.GetEntryOrFallback(conditionKey).Value;
            NodeValueAdjacentsPair<TNodeKey, TEdgeLabel, TCategory> categoryPair = categoryGraph.GetEntryOrFallback(categoryKey).Value;
            //---|

            //--- Check value staisfies condition ---
            state.CheckResult = categoryPair.NodeValue.Satisfies(in valuePair.NodeValue, in conditionPair.NodeValue);
            state.Checked = true;

            // If result is false, return false.
            if (state.CheckResult == false) {return false;}
            //---|

            //--- Visit adjacents ---
            PackedMap<TEdgeLabel, OptionalKey<TNodeKey>> conditionAdjacentsPackedMap = conditionGraph.GetAdjacentsAsPackedMap(conditionKey);
            PackedMap<TEdgeLabel, OptionalKey<TNodeKey>> categoryAdjacentsPackedMap = categoryGraph.GetAdjacentsAsPackedMap(categoryKey);
            foreach (ref readonly LabeledNode<TNodeKey, TEdgeLabel> adj in valuePair.Adjacents.Span)
            {
                TNodeKey nextValueKey = adj.Node;
                OptionalKey<TNodeKey> nextConditionKey = conditionAdjacentsPackedMap.GetEntryOrFallback(adj.Label).Value;
                OptionalKey<TNodeKey> nextCategoryKey = categoryAdjacentsPackedMap.GetEntryOrFallback(adj.Label).Value;

                // If result is false, return false.
                if (!DepthFirstCheck(in valueGraph, in conditionGraph, in categoryGraph, in statesMap, Vct.Choice2((nextValueKey, nextConditionKey, nextCategoryKey)))) {return false;}
            }
            //---|

            return true;
        }
    }

    public bool IsSufficient(scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TCondition> sufficient, scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TCondition> necessary)
    {
        PackedMap<TNodeKey, NodeKeyState> commonKeysMap = sufficient.PackedMap.SelectValue(static _ => new NodeKeyState());

        //--- First loop : Check common keys first ---
        foreach (ref RawKeyValuePair<TNodeKey, NodeKeyState> entry in commonKeysMap.AsSpan)
        {
            ref NodeKeyState curState = ref entry.Value;

            //--- Check if current node-key is already checked ---
            if (curState.DfcState.Checked)
            {
                if (!curState.DfcState.CheckResult) {return false;}
            }
            //---|

            //--- Check if current node-key is common key ---
            curState.IsCommonKey = necessary.PackedMap.ContainsKey(entry.Key) && _categoryGraph.PackedMap.ContainsKey(entry.Key);

            // if not, continue
            if (!curState.IsCommonKey) {continue;}
            //---|

            if (!DepthFirstCheck(in sufficient, in necessary, in _categoryGraph, in commonKeysMap, Vct.Choice1(entry.Key))) {return false;}
        }
        //---|

        //--- Second loop : Check unchecked node-keys ---
        foreach (ref RawKeyValuePair<TNodeKey, NodeKeyState> entry in commonKeysMap.AsSpan)
        {
            ref NodeKeyState curState = ref entry.Value;

            if (curState.DfcState.Checked) {continue;}

            if (!DepthFirstCheck(in sufficient, in necessary, in _categoryGraph, in commonKeysMap, Vct.Choice2((entry.Key, (OptionalKey<TNodeKey>)Okt.None, (OptionalKey<TNodeKey>)Okt.None)))) {return false;}
        }
        //---|

        return true;

        static bool DepthFirstCheck
        (
            scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TCondition> sufficientGraph,
            scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TCondition> necessaryGraph,
            scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TCategory> categoryGraph,
            scoped ref readonly PackedMap<TNodeKey, NodeKeyState> statesMap, 
            ValueChoice<TNodeKey, (TNodeKey ValueKey, OptionalKey<TNodeKey> ConditonKey, OptionalKey<TNodeKey> CategoryKey)> commonOrDiffrentKey
        )
        {
            (TNodeKey valueKey, OptionalKey<TNodeKey> conditionKey, OptionalKey<TNodeKey> categoryKey) = 
                commonOrDiffrentKey.Match<(TNodeKey, OptionalKey<TNodeKey>, OptionalKey<TNodeKey>)>
                (
                    caseOfChoice1: static vk => (vk, Okt.Some(vk), Okt.Some(vk)),
                    caseOfChoice2: static pair => pair
                );

            ref DfcState state = ref statesMap.GetValueRef(valueKey).DfcState;
            if (state.Checked) {return state.CheckResult;}

            //--- Get pairs of value, conditon, category ---
            NodeValueAdjacentsPair<TNodeKey, TEdgeLabel, TCondition> sufficientPair = sufficientGraph.GetEntryOrFallback(valueKey).Value;
            NodeValueAdjacentsPair<TNodeKey, TEdgeLabel, TCondition> necessaryPair = necessaryGraph.GetEntryOrFallback(conditionKey).Value;
            NodeValueAdjacentsPair<TNodeKey, TEdgeLabel, TCategory> categoryPair = categoryGraph.GetEntryOrFallback(categoryKey).Value;
            //---|

            //--- Check value staisfies condition ---
            state.CheckResult = categoryPair.NodeValue.IsSufficient(in sufficientPair.NodeValue, in necessaryPair.NodeValue);
            state.Checked = true;

            // If result is false, return false.
            if (state.CheckResult == false) {return false;}
            //---|

            //--- Visit adjacents ---
            PackedMap<TEdgeLabel, OptionalKey<TNodeKey>> conditionAdjacentsPackedMap = necessaryGraph.GetAdjacentsAsPackedMap(conditionKey);
            PackedMap<TEdgeLabel, OptionalKey<TNodeKey>> categoryAdjacentsPackedMap = categoryGraph.GetAdjacentsAsPackedMap(categoryKey);
            foreach (ref readonly LabeledNode<TNodeKey, TEdgeLabel> adj in sufficientPair.Adjacents.Span)
            {
                TNodeKey nextValueKey = adj.Node;
                OptionalKey<TNodeKey> nextConditionKey = conditionAdjacentsPackedMap.GetEntryOrFallback(adj.Label).Value;
                OptionalKey<TNodeKey> nextCategoryKey = categoryAdjacentsPackedMap.GetEntryOrFallback(adj.Label).Value;

                // If result is false, return false.
                if (!DepthFirstCheck(in sufficientGraph, in necessaryGraph, in categoryGraph, in statesMap, Vct.Choice2((nextValueKey, nextConditionKey, nextCategoryKey)))) {return false;}
            }
            //---|

            return true;
        }
    }
}
