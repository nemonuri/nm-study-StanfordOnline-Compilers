
namespace DscTool.Scoped.Sequences;

public readonly struct ScopedReadOnlyMemoryCategory
    <T, TCondition, TEqualityComparer, TChecker, TSufficientChecker> :
    IScopedCategory<ReadOnlyMemory<T>, ReadOnlyMemory<TCondition>>
    where TEqualityComparer : IEqualityComparer<T>
    where TChecker : IScopedConditionChecker<T, TCondition>
    where TSufficientChecker : IScopedSufficientChecker<TCondition>
{
    private readonly ReadOnlyMemoryEqualityComparer<T, TEqualityComparer> _equalityComparer;
    private readonly ScopedReadOnlyMemoryConditionChecker<T, TCondition, TChecker> _checker;
    private readonly ScopedReadOnlyMemorySufficientChecker<TCondition, TSufficientChecker> _sufficientChecker;

    public ScopedReadOnlyMemoryCategory
    (
        ReadOnlyMemory<TEqualityComparer> equalityComparer, 
        ReadOnlyMemory<TChecker> checker, 
        ReadOnlyMemory<TSufficientChecker> sufficientChecker
    )
    {
        _equalityComparer = new(equalityComparer);
        _checker = new(checker);
        _sufficientChecker = new(sufficientChecker);
    }

    public bool Equals(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y) => _equalityComparer.Equals(x, y);

    public int GetHashCode(ReadOnlyMemory<T> obj) => _equalityComparer.GetHashCode(obj);

    public bool Satisfies(scoped ref readonly ReadOnlyMemory<T> value, scoped ref readonly ReadOnlyMemory<TCondition> condition) =>
        _checker.Satisfies(in value, in condition);

    public bool IsSufficient(scoped ref readonly ReadOnlyMemory<TCondition> sufficient, scoped ref readonly ReadOnlyMemory<TCondition> necessary) =>
        _sufficientChecker.IsSufficient(in sufficient, in necessary);
}

public readonly struct ScopedReadOnlyMemoryCategory<T, TCondition, TCategory> :
    IScopedCategory<ReadOnlyMemory<T>, ReadOnlyMemory<TCondition>>
    where TCategory : IScopedCategory<T, TCondition>
{
    private readonly ScopedReadOnlyMemoryCategory<T, TCondition, TCategory, TCategory, TCategory> _category;

    public ScopedReadOnlyMemoryCategory(ReadOnlyMemory<TCategory> categories)
    {
        _category = new(categories, categories, categories);
    }

    public bool Equals(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y) => _category.Equals(x, y);

    public int GetHashCode(ReadOnlyMemory<T> obj) => _category.GetHashCode(obj);

    public bool Satisfies(scoped ref readonly ReadOnlyMemory<T> value, scoped ref readonly ReadOnlyMemory<TCondition> condition) =>
        _category.Satisfies(in value, in condition);

    public bool IsSufficient(scoped ref readonly ReadOnlyMemory<TCondition> sufficient, scoped ref readonly ReadOnlyMemory<TCondition> necessary) =>
        _category.IsSufficient(in sufficient, in necessary);
}

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
