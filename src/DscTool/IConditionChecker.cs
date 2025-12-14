namespace DscTool;

public interface IConditionChecker<T, TCondition>
{
    bool Satisfies(scoped ref readonly T value, scoped ref readonly TCondition condition);
}

public interface ISufficientChecker<TCondition>
{
    bool IsSufficient(scoped ref readonly TCondition sufficient, scoped ref readonly TCondition necessary);
}
