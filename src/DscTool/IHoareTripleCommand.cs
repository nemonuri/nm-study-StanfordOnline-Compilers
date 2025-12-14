namespace DscTool;

public interface IHoareTripleCommand<T, TCondition>
{
    [UnscopedRef] ref readonly TCondition PreCondition {get;}

    T Invoke(scoped ref readonly T value, out TCondition postCondition);
}

public interface IHoareTripleMorphism<TSource, TSourcePredicate, TTarget, TTargetPredicate>
{
    [UnscopedRef] ref readonly TSourcePredicate PreCondition {get;}

    TTarget Morph(scoped ref readonly TSource source, out TTargetPredicate postCondition);
}

