namespace DscTool.Scoped.Hashtables;

public interface IScopedCategoryProvider<T, TCondition, TCategory>
    where TCategory : IScopedCategory<T, TCondition>
{
    bool TryGetCategory
    (
        scoped ref readonly TCondition condition,
        [NotNullWhen(true)] scoped ref TCategory? category
    );
}

public interface IScopedCategoryProvidableDictionary<T, TCondition, TCategory> : 
    IValueEntryReadOnlyDictionary<T, TCondition>,
    IScopedCategoryProvider<T, TCondition, TCategory>
    where TCategory : IScopedCategory<T, TCondition>
{
}
