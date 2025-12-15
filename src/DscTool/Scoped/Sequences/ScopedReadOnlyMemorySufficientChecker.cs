
namespace DscTool.Scoped.Sequences;

public readonly struct ScopedReadOnlyMemorySufficientChecker<TCondition, TSufficientChecker> :
    IScopedSufficientChecker<ReadOnlyMemory<TCondition>>
    where TSufficientChecker : IScopedSufficientChecker<TCondition>
{
    private readonly TSufficientChecker _sufficientChecker;

    public ScopedReadOnlyMemorySufficientChecker(TSufficientChecker sufficientChecker)
    {
        _sufficientChecker = sufficientChecker;
    }

    public bool IsSufficient
    (
        scoped ref readonly ReadOnlyMemory<TCondition> sufficient, 
        scoped ref readonly ReadOnlyMemory<TCondition> necessary
    )
    {
        scoped var sufficients = sufficient.Span;
        scoped var necessaries = necessary.Span;

        int length = sufficients.Length;
        if (length != necessaries.Length) {return false;}

        for (int i = 0; i < length; i++)
        {
            if (!_sufficientChecker.IsSufficient(in sufficients[i], in necessaries[i])) {return false;}
        }
        return true;
    }
}
