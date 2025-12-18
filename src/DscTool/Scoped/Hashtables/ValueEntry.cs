using DscTool.Infrastructure;

namespace DscTool.Scoped.Hashtables;

public readonly struct ValueEntry<T, TCondition, TCategory>
(
    ImmutableArray<T> heads, 
    TCondition condition,
    TCategory category
)
    where TCategory : IScopedCategory<T, TCondition>
{
    public readonly ImmutableArray<T> Heads = heads;
    public readonly TCondition Condition = condition;
    public readonly TCategory Category = category;
}

/*
public interface IScopedCategoryProvider<T, TCondition, TCategory>
    where TCategory : IScopedCategory<T, TCondition>
{
    bool TryGetCategory
}
*/

public interface IValueEntryReadOnlyDictionary<T, TCondition, TCategory> : 
    IReadOnlyMemoryLikeDictionary<T, ValueEntry<T, TCondition, TCategory>>
    where TCategory : IScopedCategory<T, TCondition>
{
}

public interface IValueEntryDictionary<T, TCondition, TCategory> : 
    IMemoryLikeDictionary<T, ValueEntry<T, TCondition, TCategory>>
    where TCategory : IScopedCategory<T, TCondition>
{
}
