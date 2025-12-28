namespace Nemonuri.LowLevel;

public interface ICommandProxy<TState, TEvent, TArg, TError, TConfig>
{
    bool CheckPreCondition(scoped in StateProxy<TState, TEvent, TConfig> state, out AbstractMemoryView<TConfig, TError> errorProxies);

    bool InvokeAndCheckPostCondition
    (
        scoped in StateProxy<TState, TEvent, TConfig> state, 
        scoped in TArg arg,
        out AbstractMemoryView<TConfig, TEvent> successEvents,
        out AbstractMemoryView<TConfig, TError> errorProxies
    );
}
