
using DscTool.Infrastructure;

namespace DscTool.Scoped.Hashtables;

public readonly partial struct ScopedPackedMapCategoriedCommand<T, TCondition, TCategory, TCategoriedCommand, TKey> :
    IScopedCategoriedCommand<
        PackedMap<TKey, T>, 
        PackedMap<TKey, TCondition>, 
        ScopedPackedMapCategory<T, TCondition, TCategory, TKey>>
    where TCategory : IScopedCategory<T, TCondition>
    where TCategoriedCommand : IScopedCategoriedCommand<T, TCondition, TCategory>
    where TKey : IEquatable<TKey>
{
    private readonly ScopedPackedMapHoareTripleCommand<T, TCondition, TCategoriedCommand, TKey> _command;
    private readonly PackedMap<TKey, TCondition> _precondition;
    private readonly ScopedPackedMapCategory<T, TCondition, TCategory, TKey> _category;

    public ScopedPackedMapCategoriedCommand
    (
        PackedMap<TKey, TCategoriedCommand> categoriedCommand
    )
    {
        _command = new(categoriedCommand);
        _precondition = _command.PreCondition;
        _category = new(categoriedCommand.SelectValue(static c => c.Category));
    }

    [UnscopedRef] public ref readonly ScopedPackedMapCategory<T, TCondition, TCategory, TKey> Category => ref _category;

    [UnscopedRef] public ref readonly PackedMap<TKey, TCondition> PreCondition => ref _precondition;

    public bool TryInvoke
    (
        scoped ref readonly PackedMap<TKey, T> source, 
        [NotNullWhen(true)] scoped ref PackedMap<TKey, T> result, 
        [NotNullWhen(true)] scoped ref PackedMap<TKey, TCondition> postCondition
    )
    {
        return _command.TryInvoke(in source, ref result, ref postCondition);
    }
}
