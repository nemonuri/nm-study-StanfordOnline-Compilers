namespace DscTool;

public interface IHoareTripleCommand<T, TPredicate>
{
    [UnscopedRef] ref readonly TPredicate PreCondition {get;}

    T Invoke(scoped ref readonly T value, out TPredicate postCondition);
}

public interface IHoareTripleMorphism<TSource, TSourcePredicate, TTarget, TTargetPredicate>
{
    [UnscopedRef] ref readonly TSourcePredicate PreCondition {get;}

    TTarget Morph(scoped ref readonly TSource source, out TTargetPredicate postCondition);
}
