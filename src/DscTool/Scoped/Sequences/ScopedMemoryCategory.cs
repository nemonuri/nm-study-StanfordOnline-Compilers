
namespace DscTool.Scoped.Sequences;

public readonly struct ScopedMemoryCategory<T, TCondition, TCategory> :
    IScopedCategory<Memory<T>, Memory<TCondition>>
    where TCategory : IScopedCategory<T, TCondition>
{
    private readonly ScopedReadOnlyMemoryCategory<T, TCondition, TCategory> _category;

    public ScopedMemoryCategory(ReadOnlyMemory<TCategory> categories)
    {
        _category = new(categories);
    }

    public bool Equals(Memory<T> x, Memory<T> y) => _category.Equals(x, y);

    public int GetHashCode(Memory<T> obj) => _category.GetHashCode(obj);

    public bool Satisfies(scoped ref readonly Memory<T> value, scoped ref readonly Memory<TCondition> condition)
    {
        ReadOnlyMemory<T> roValue = value;
        ReadOnlyMemory<TCondition> roCondition = condition;
        return _category.Satisfies(in roValue, in roCondition);
    }

    public bool IsSufficient(scoped ref readonly Memory<TCondition> sufficient, scoped ref readonly Memory<TCondition> necessary)
    {
        ReadOnlyMemory<TCondition> roSufficient = sufficient;
        ReadOnlyMemory<TCondition> roNecessary = necessary;
        return _category.IsSufficient(in roSufficient, in roNecessary);
    }
}



