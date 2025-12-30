namespace Nemonuri.LowLevel;

public readonly struct Situation<TState, TEvent, TConfig>
{
    public readonly TState SnapShot;
    public readonly MemoryViewReceiver<TConfig, TEvent> History;

    public Situation(TState snapShot, MemoryViewReceiver<TConfig, TEvent> history)
    {
        SnapShot = snapShot;
        History = history;
    }
}
