namespace DscTool.Scoped;

public interface ISupportGetPreCondition<TCondition>
{
    ref readonly TCondition PreCondition {get;}
}

public interface IScopedHoareTripleCommand<T, TCondition> : ISupportGetPreCondition<TCondition>
{
    bool TryInvoke
    (
        scoped ref readonly T source, 
        [NotNullWhen(true)] scoped ref T? result, 
        [NotNullWhen(true)] scoped ref TCondition? postCondition
    );
}

public interface IScopedHoareTripleMorphism<TSource, TSourceCondition, TTarget, TTargetCondition> : ISupportGetPreCondition<TSourceCondition>
{
    bool TryMorph
    (
        scoped ref readonly TSource source, 
        [NotNullWhen(true)] scoped ref TTarget? result, 
        [NotNullWhen(true)] scoped ref TTargetCondition? postCondition
    );
}

