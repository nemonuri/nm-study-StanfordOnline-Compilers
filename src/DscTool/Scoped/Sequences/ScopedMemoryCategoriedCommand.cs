
namespace DscTool.Scoped.Sequences;

public readonly struct ScopedMemoryCategoriedCommand<T, TCondition, TCategory, TCategoriedCommand> :
    IScopedCategoriedCommand<ReadOnlyMemory<T>, ReadOnlyMemory<TCondition>, ScopedReadOnlyMemoryCategory<T, TCondition, TCategory>>
    where TCategory : IScopedCategory<T, TCondition>
    where TCategoriedCommand : IScopedCategoriedCommand<T, TCondition, TCategory>
{
    private readonly ScopedReadOnlyMemoryCategory<T, TCondition, TCategory> _category;

    public ref readonly ReadOnlyMemory<TCondition> PreCondition => throw new NotImplementedException();

    [UnscopedRef] public ref readonly ScopedReadOnlyMemoryCategory<T, TCondition, TCategory> Category => ref _category;

    public ScopedMemoryCategoriedCommand(TCategory category)
    {
        _category = new(category);
    }

    public bool TryInvoke
    (
        scoped ref readonly ReadOnlyMemory<T> source, 
        [NotNullWhen(true)] scoped ref ReadOnlyMemory<T> result, 
        [NotNullWhen(true)] scoped ref ReadOnlyMemory<TCondition> postCondition
    )
    {
        throw new NotImplementedException();
    }


}

