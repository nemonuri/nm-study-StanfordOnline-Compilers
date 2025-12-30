namespace Nemonuri.LowLevel;

public interface ICommandProxy<TState, TEvent, TArgument, TError, TReceiver>
{
    bool TryInvoke
    (
        scoped in Situation<TState, TEvent, TReceiver> situation, 
        scoped in TArgument? argument,
        [NotNullWhen(true)] out TEvent? successEvent,
        [NotNullWhen(false)] out TError? failError
    );

    bool CanSolveError
    (
        scoped in Situation<TState, TEvent, TReceiver> situation,
        scoped in TError error,
        [NotNullWhen(true)] out TArgument? solutionArgument
    );

    //LowLevelComparerReceiver<TReceiver, TArgument> 
}

public readonly struct InvocationInfo<TCommandId, TArgument>(TCommandId commandId, TArgument argument)
    where TCommandId : IEquatable<TCommandId>
{
    public readonly TCommandId CommandId = commandId;
    public readonly TArgument? Argument = argument;
}

public readonly struct CommandInvocationFrame<TState, TEvent, TArgument, TProblem, TCommandId, TCommand, TReceiver>
    where TCommandId : IEquatable<TCommandId>
    where TCommand : ICommandProxy<TState, TEvent, TArgument, TProblem, TReceiver>
{
    public readonly LowLevelTableReceiver<TReceiver, TCommandId, TCommand> Commands;
    public readonly InvocationInfo<TCommandId, TArgument> InitialInvocation;
    public readonly IComparer<TCommandId> CommandPriorityComparer;
}
