
namespace DscTool.Scoped.Hashtables;

public readonly struct ValueEntry<T, TCondition>
(
    ImmutableArray<T> heads, 
    TCondition condition
)
{
    public readonly ImmutableArray<T> Heads = heads;
    public readonly TCondition Condition = condition;
}


public interface IValueEntryReadOnlyDictionary<T, TCondition /*, TCategory*/> : 
    IReadOnlyDictionary<T, ValueEntry<T, TCondition>>
    //IReadOnlyMemoryLikeDictionary<T, ValueEntry<T, TCondition>>
    //where TCategory : IScopedCategory<T, TCondition>
{
}

public interface IValueEntryDictionary<T, TCondition /*, TCategory*/> : 
    //IMemoryLikeDictionary<T, ValueEntry<T, TCondition>>
    IDictionary<T, ValueEntry<T, TCondition>>
    //where TCategory : IScopedCategory<T, TCondition>
{
}
