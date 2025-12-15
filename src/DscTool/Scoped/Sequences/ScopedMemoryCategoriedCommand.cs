
namespace DscTool.Scoped.Sequences;

public readonly struct ScopedMemoryCategoriedCommand<T, TCondition, TCategory, TCategoriedCommand> :
    IScopedCategoriedCommand<Memory<T>, Memory<TCondition>, ScopedMemoryCategory<T, TCondition, TCategory>>
    where TCategory : IScopedCategory<T, TCondition>
    where TCategoriedCommand : IScopedCategoriedCommand<T, TCondition, TCategory>
{
    private readonly ScopedMemoryCategory<T, TCondition, TCategory> _category;

    private readonly ScopedMemoryHoareTripleCommand<T, TCondition, TCategoriedCommand> _command;
    private readonly Memory<TCondition> _precondition;

    [UnscopedRef] public ref readonly Memory<TCondition> PreCondition => ref _precondition;

    [UnscopedRef] public ref readonly ScopedMemoryCategory<T, TCondition, TCategory> Category => ref _category;

    public ScopedMemoryCategoriedCommand(ReadOnlyMemory<TCategoriedCommand> commands)
    {
        TCategory[] categories = new TCategory[commands.Length];
        scoped var commandsSpan = commands.Span;
        for (int i = 0; i < commandsSpan.Length; i++)
        {
            categories[i] = commandsSpan[i].Category;
        }

        _category = new(categories);
        _command = new(commands);
        _precondition = _command.PreCondition;
    }

    public bool TryInvoke
    (
        scoped ref readonly Memory<T> source, 
        [NotNullWhen(true)] scoped ref Memory<T> result, 
        [NotNullWhen(true)] scoped ref Memory<TCondition> postCondition
    )
    {
        return _command.TryInvoke(in source, ref result, ref postCondition);
    }
}
