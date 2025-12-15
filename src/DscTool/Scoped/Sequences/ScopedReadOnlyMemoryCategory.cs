
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

    public ScopedReadOnlyMemoryCategory(TEqualityComparer equalityComparer, TChecker checker, TSufficientChecker sufficientChecker)
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

    public ScopedReadOnlyMemoryCategory(TCategory category)
    {
        _category = new(category, category, category);
    }

    public bool Equals(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y) => _category.Equals(x, y);

    public int GetHashCode(ReadOnlyMemory<T> obj) => _category.GetHashCode(obj);

    public bool Satisfies(scoped ref readonly ReadOnlyMemory<T> value, scoped ref readonly ReadOnlyMemory<TCondition> condition) =>
        _category.Satisfies(in value, in condition);

    public bool IsSufficient(scoped ref readonly ReadOnlyMemory<TCondition> sufficient, scoped ref readonly ReadOnlyMemory<TCondition> necessary) =>
        _category.IsSufficient(in sufficient, in necessary);
}
