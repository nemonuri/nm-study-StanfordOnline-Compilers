
namespace DscTool.Scoped.Sequences;

public readonly struct ScopedMemoryHoareTripleCommand<T, TCondition, TCommand> : 
    IScopedHoareTripleCommand<Memory<T>, Memory<TCondition>>
    where TCommand : IScopedHoareTripleCommand<T, TCondition>
{
    private readonly ReadOnlyMemory<TCommand> _commands;
    private readonly Memory<TCondition> _preconditions;
    
    [UnscopedRef] public ref readonly Memory<TCondition> PreCondition => ref _preconditions;

    public ScopedMemoryHoareTripleCommand(ReadOnlyMemory<TCommand> commands)
    {
        _commands = commands;
        scoped var commandsSpan = _commands.Span;
        var conditions = new TCondition[_commands.Length];
        for (int i = 0; i < conditions.Length; i++)
        {
            conditions[i] = commandsSpan[i].PreCondition;
        }
        _preconditions = conditions;
    }

    public bool TryInvoke
    (
        scoped ref readonly Memory<T> source, 
        [NotNullWhen(true)] scoped ref Memory<T> result, 
        [NotNullWhen(true)] scoped ref Memory<TCondition> postCondition
    )
    {
        scoped var sources = source.Span;
        int length = source.Length;
        scoped var results = result.Span;
        scoped var postConditions = postCondition.Span;
        scoped var commandsSpan = _commands.Span;

        if (length != result.Length) {return false;}
        if (length != postCondition.Length) {return false;}
        for (int i = 0; i < length; i++)
        {
            if (!commandsSpan[i].TryInvoke(in sources[i], ref results[i]!, ref postConditions[i]!)) {return false;}
        }
        return true;
    }
}
