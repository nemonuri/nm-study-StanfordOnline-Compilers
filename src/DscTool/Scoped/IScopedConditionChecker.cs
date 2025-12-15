namespace DscTool.Scoped;

public interface IScopedConditionChecker<T, TCondition>
{
    bool Satisfies(scoped ref readonly T value, scoped ref readonly TCondition condition);
}

public interface IScopedSufficientChecker<TCondition>
{
    bool IsSufficient(scoped ref readonly TCondition sufficient, scoped ref readonly TCondition necessary);
}

public interface IScopedEqualityChecker<T>
{
    bool Equals(scoped ref readonly T x, scoped ref readonly T y);

    int GetHashCode(scoped ref readonly T obj);
}
