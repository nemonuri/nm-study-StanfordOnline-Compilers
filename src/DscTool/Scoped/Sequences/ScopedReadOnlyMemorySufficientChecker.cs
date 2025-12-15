
namespace DscTool.Scoped.Sequences;

public readonly struct ScopedReadOnlyMemorySufficientChecker<TCondition, TSufficientChecker> :
    IScopedSufficientChecker<ReadOnlyMemory<TCondition>>
    where TSufficientChecker : IScopedSufficientChecker<TCondition>
{
    private readonly ReadOnlyMemory<TSufficientChecker> _sufficientChecker;

    public ScopedReadOnlyMemorySufficientChecker(ReadOnlyMemory<TSufficientChecker> sufficientCheckers)
    {
        _sufficientChecker = sufficientCheckers;
    }

    public bool IsSufficient
    (
        scoped ref readonly ReadOnlyMemory<TCondition> sufficient, 
        scoped ref readonly ReadOnlyMemory<TCondition> necessary
    )
    {
        scoped var sufficients = sufficient.Span;
        scoped var necessaries = necessary.Span;
        scoped var sufficientCheckers = _sufficientChecker.Span;

        int length = sufficients.Length;
        if (length != necessaries.Length) {return false;}
        if (length != sufficientCheckers.Length) {return false;}

        for (int i = 0; i < length; i++)
        {
            if (!sufficientCheckers[i].IsSufficient(in sufficients[i], in necessaries[i])) {return false;}
        }
        return true;
    }
}
