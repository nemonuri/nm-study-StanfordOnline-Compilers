using DscTool.Infrastructure;

namespace DscTool.Scoped.Graphs;

public readonly struct ScopedPackedGraphTestCommand<T, TCondition, TCategory, TCommand, TNodeKey, TEdgeLabel> :
    IScopedCategoriedCommand<
        PackedDigraph<TNodeKey, TEdgeLabel, TestInfo<T, TCondition>>,
        PackedDigraph<TNodeKey, TEdgeLabel, TCondition>,
        ScopedPackedGraphCategory<TestInfo<T, TCondition>, TCondition, TestInfoCategory<T, TCondition, TCategory>, TNodeKey, TEdgeLabel>>
    where TCategory : IScopedCategory<T, TCondition>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
    where TCommand : IScopedCategoriedCommand<T, TCondition, TCategory>
{
    private readonly PackedDigraph<TNodeKey, TEdgeLabel, TCategory> _categoryGraph;
    private readonly PackedDigraph<TNodeKey, TEdgeLabel, TCondition> _precondition;
    private readonly ScopedPackedGraphCategory<TestInfo<T, TCondition>, TCondition, TestInfoCategory<T, TCondition, TCategory>, TNodeKey, TEdgeLabel> _categoryGraph2;

    [UnscopedRef] public ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TCondition> PreCondition => ref _precondition;

    [UnscopedRef] public ref readonly ScopedPackedGraphCategory<TestInfo<T, TCondition>, TCondition, TestInfoCategory<T, TCondition, TCategory>, TNodeKey, TEdgeLabel> Category => ref _categoryGraph2;

    public ScopedPackedGraphTestCommand(PackedDigraph<TNodeKey, TEdgeLabel, TCategory> categoryGraph, TCondition weakestCondition)
    {
        _categoryGraph = categoryGraph;
        _precondition = categoryGraph.SelectValue(_ => weakestCondition);
        _categoryGraph2 = new(categoryGraph.SelectValue(static c => new TestInfoCategory<T, TCondition, TCategory>(c)));
    }

    public bool TryInvoke
    (
        scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TestInfo<T, TCondition>> source, 
        [NotNullWhen(true)] scoped ref PackedDigraph<TNodeKey, TEdgeLabel, TestInfo<T, TCondition>> result, 
        [NotNullWhen(true)] scoped ref PackedDigraph<TNodeKey, TEdgeLabel, TCondition> postCondition
    )
    {
        var resultCandidate = source.SelectValue(static a => a);
        var sourceSpan = source.AsSpan;
        var resultSpan = result.AsSpan;
        for (int i=0;i < sourceSpan.Length;i++)
        {
            ref readonly var sourceEntry = ref sourceSpan[i];
            ref readonly var sourceTestInfo = ref sourceEntry.Value.NodeValue;
            bool success = _categoryGraph.GetEntryOrFallback(sourceEntry.Key).Value.NodeValue.Satisfies(in sourceTestInfo.Value, in sourceTestInfo.Condition);
            resultSpan[i].Value = new NodeValueAdjacentsPair<TNodeKey, TEdgeLabel, TestInfo<T, TCondition>>(nodeValue: new(sourceTestInfo.Value, sourceTestInfo.Condition, success), adjacents: sourceEntry.Value.Adjacents);
        }
        result = resultCandidate;
        postCondition = _precondition;
        return true;
    }

    
}
