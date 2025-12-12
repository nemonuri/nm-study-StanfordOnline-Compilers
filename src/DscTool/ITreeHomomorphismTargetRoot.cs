namespace DscTool;

public interface ITreeHomomorphismTargetRoot<TSource, TTarget>
{
    bool TryMorph
    (
        scoped ref readonly TSource sourceRoot, 
        int sourceChildIndex, 
        [NotNullWhen(true)] out TSource? sourceResult,
        [NotNullWhen(true)] out TTarget? targetResult, 
        out TreeMorphErrorKind errorKind
    );
}
