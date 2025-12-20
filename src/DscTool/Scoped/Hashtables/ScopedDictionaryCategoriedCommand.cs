using DscTool.Infrastructure;

namespace DscTool.Scoped.Hashtables;

public readonly partial struct ScopedDictionaryCategoriedCommand<T, TCondition, TCategory, TCategoriedCommand, TKey> :
    IScopedCategoriedCommand<
        Memory<KeyValuePair<TKey, T>>, 
        ReadOnlyDictionaryFallbackPair<TKey, TCondition>, 
        ScopedDictionaryCategory<T, TCondition, TCategory, TKey>>
    where TCategory : IScopedCategory<T, TCondition>
    where TCategoriedCommand : IScopedCategoriedCommand<T, TCondition, TCategory>
    where TKey : IEquatable<TKey>
{
    private readonly ScopedDictionaryHoareTripleCommand<T, TCondition, TCategoriedCommand, TKey> _command;

    private readonly ScopedDictionaryCategory<T, TCondition, TCategory, TKey> _category;

    public ScopedDictionaryCategoriedCommand
    (
        ReadOnlyDictionaryFallbackPair<TKey, TCategoriedCommand> categoriedCommand
    )
    {
        _command = new(categoriedCommand);
        _category = new(new ReadOnlyDictionaryFallbackPair<TKey, TCategory>(new CategoryTable(categoriedCommand.Dictionary), categoriedCommand.Fallback.Category));
    }

    [UnscopedRef] public ref readonly ScopedDictionaryCategory<T, TCondition, TCategory, TKey> Category => ref _category;

    [UnscopedRef] public ref readonly ReadOnlyDictionaryFallbackPair<TKey, TCondition> PreCondition => ref _command.PreCondition;

    public bool TryInvoke
    (
        scoped ref readonly Memory<KeyValuePair<TKey, T>> source, 
        [NotNullWhen(true)] scoped ref Memory<KeyValuePair<TKey, T>> result, 
        [NotNullWhen(true)] scoped ref ReadOnlyDictionaryFallbackPair<TKey, TCondition> postCondition
    )
    {
        return _command.TryInvoke(in source, ref result, ref postCondition);
    }
}
