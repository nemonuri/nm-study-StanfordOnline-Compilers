
namespace DscTool.Scoped.Sequences;

public readonly struct ScopedReadOnlyMemoryConditionChecker<T, TCondition, TChecker> :
    IScopedConditionChecker<ReadOnlyMemory<T>, ReadOnlyMemory<TCondition>>
    where TChecker : IScopedConditionChecker<T, TCondition>
{
    private readonly TChecker _checker;

    public ScopedReadOnlyMemoryConditionChecker(TChecker checker)
    {
        _checker = checker;
    }

    public bool Satisfies(scoped ref readonly ReadOnlyMemory<T> value, scoped ref readonly ReadOnlyMemory<TCondition> condition)
    {
        scoped var valueSpan = value.Span;
        scoped var conditionSpan = condition.Span;

        int length = valueSpan.Length;
        if (length != condition.Length) { return false; }

        for (int i = 0; i < length; i++)
        {
            if (!_checker.Satisfies(in valueSpan[i], in conditionSpan[i])) {return false;}
        }
        return true;
    }
}

