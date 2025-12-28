namespace Nemonuri.LowLevel;

public readonly struct StateProxy<TState, TEvent, TConfig>
{
    public readonly TState SnapShot;
    public readonly AbstractMemoryView<TConfig, TEvent> History;

    public StateProxy(TState snapShot, AbstractMemoryView<TConfig, TEvent> history)
    {
        SnapShot = snapShot;
        History = history;
    }
}
