namespace DscTool.Scoped.Graphs;

public readonly struct TestInfo<T, TCondition>
{
    public readonly T Value;
    public readonly TCondition Condition;
    public readonly bool Result;

    public TestInfo(T value, TCondition condition, bool result)
    {
        Value = value;
        Condition = condition;
        Result = result;
    }
}

public readonly struct TestInfoCategory<T, TCondition, TCategory> : 
    IScopedCategory<TestInfo<T, TCondition>, TCondition>
    where TCategory : IScopedCategory<T, TCondition>
{
    private readonly TCategory _category;

    public TestInfoCategory(TCategory category)
    {
        _category = category;
    }

    public bool Equals(TestInfo<T, TCondition> x, TestInfo<T, TCondition> y)
    {
        return _category.Equals(x.Value, y.Value);
    }

    public int GetHashCode(TestInfo<T, TCondition> obj)
    {
        return _category.GetHashCode(obj.Value);
    }

    public bool Satisfies(scoped ref readonly TestInfo<T, TCondition> value, scoped ref readonly TCondition condition)
    {
        return _category.Satisfies(in value.Value, in condition);
    }

    public bool IsSufficient(scoped ref readonly TCondition sufficient, scoped ref readonly TCondition necessary)
    {
        return _category.IsSufficient(in sufficient, in necessary);
    }
}