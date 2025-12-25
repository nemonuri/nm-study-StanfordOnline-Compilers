using DscTool.Infrastructure;

namespace DscTool.Scoped.Graphs;

public readonly struct ScopedPackedGraphCategoriedCommand<T, TCondition, TCategory, TCommand, TNodeKey, TEdgeLabel> :
    IScopedCategoriedCommand<
        PackedDigraph<TNodeKey, TEdgeLabel, T>,
        PackedDigraph<TNodeKey, TEdgeLabel, TCondition>,
        ScopedPackedGraphCategory<T, TCondition, TCategory, TNodeKey, TEdgeLabel>>
    where TCategory : IScopedCategory<T, TCondition>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
    where TCommand : IScopedCategoriedCommand<T, TCondition, TCategory>
{
    private readonly ScopedPackedGraphCategory<T, TCondition, TCategory, TNodeKey, TEdgeLabel> _category;

    private readonly ScopedPackedGraphHoareTripleCommand<T, TCondition, TCommand, TNodeKey, TEdgeLabel> _command;

    public ScopedPackedGraphCategoriedCommand(PackedDigraph<TNodeKey, TEdgeLabel, TCommand> commandGraph)
    {
        _category = new(commandGraph.SelectValue(static v => v.Category));
        _command = new(commandGraph);
    }

    [UnscopedRef] public ref readonly ScopedPackedGraphCategory<T, TCondition, TCategory, TNodeKey, TEdgeLabel> Category => ref _category;

    [UnscopedRef] public ref readonly PackedDigraph<TNodeKey, TEdgeLabel, TCondition> PreCondition => ref _command.PreCondition;

    public bool TryInvoke
    (
        scoped ref readonly PackedDigraph<TNodeKey, TEdgeLabel, T> source, 
        [NotNullWhen(true)] scoped ref PackedDigraph<TNodeKey, TEdgeLabel, T> result, 
        [NotNullWhen(true)] scoped ref PackedDigraph<TNodeKey, TEdgeLabel, TCondition> postCondition
    )
    {
        return _command.TryInvoke(in source, ref result, ref postCondition);
    }
}
