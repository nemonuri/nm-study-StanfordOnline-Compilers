namespace DscTool.Scoped;

public interface IScopedMorphism<TSource, TTarget>
{
    bool TryMorph(scoped ref readonly TSource source, [NotNull] scoped ref TTarget? result);
}

public interface IScopedMorphism<TSource1, TSource2, TTarget1, TTarget2>
{
    bool TryMorph
    (
        scoped ref readonly TSource1 source1, 
        scoped ref readonly TSource2 source2, 
        [NotNull] scoped ref TTarget1? result1,
        [NotNull] scoped ref TTarget2? result2
    );
}