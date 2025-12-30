namespace Nemonuri.LowLevel;

public interface ICommandProxy<TState, TEvent, TArgument, TProblem, TConfig>
{
    bool TryInvoke
    (
        scoped in Situation<TState, TEvent, TConfig> situation, 
        scoped in TArgument argument,
        [NotNullWhen(true)] out TEvent? successEvent,
        [NotNullWhen(false)] out TProblem? failProblem
    );
}

public readonly struct InvocationInfo<TCommandId, TArgument>(TCommandId commandId, TArgument argument)
    where TCommandId : IEquatable<TCommandId>
{
    public readonly TCommandId CommandId = commandId;
    public readonly TArgument Argument = argument;
}

public readonly struct CommandInvocationFrame<TState, TEvent, TArgument, TProblem, TCommandId, TCommand, TConfig>
    where TCommandId : IEquatable<TCommandId>
    where TCommand : ICommandProxy<TState, TEvent, TArgument, TProblem, TConfig>
{
    public readonly AbstractLowLevelTable<TConfig, TCommandId, TCommand> Commands;
    public readonly InvocationInfo<TCommandId, TArgument> InitialInvocation;
    public readonly LowLevelComparerHandle<InvocationInfo<TCommandId, TArgument>> InvocationInfoComparer;
}
