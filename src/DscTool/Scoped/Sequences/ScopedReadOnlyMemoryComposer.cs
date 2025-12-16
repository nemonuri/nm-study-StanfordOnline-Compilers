
namespace DscTool.Scoped.Sequences;

public readonly struct ScopedReadOnlyMemoryComposeMorphism<
    TSource, TSourceCondition, TTarget, TTargetCondition, 
    TComposer> :
    IScopedHoareTripleMorphism<ReadOnlyMemory<TSource>, ReadOnlyMemory<TSourceCondition>, TTarget, TTargetCondition>
    where TComposer : IScopedReadOnlyMemoryComposer<TSource, TSourceCondition, TTarget, TTargetCondition>
{
    private readonly ReadOnlyMemory<TSourceCondition> _precondition;
    private readonly TComposer _composer;

    public ScopedReadOnlyMemoryComposeMorphism(ReadOnlyMemory<TSourceCondition> precondition, TComposer composer)
    {
        _precondition = precondition;
        _composer = composer;
    }

    [UnscopedRef] public ref readonly ReadOnlyMemory<TSourceCondition> PreCondition => ref _precondition;

    public bool TryMorph
    (
        scoped ref readonly ReadOnlyMemory<TSource> source, 
        [NotNullWhen(true)] scoped ref TTarget? result, 
        [NotNullWhen(true)] scoped ref TTargetCondition? postCondition
    )
    {
        return _composer.TryMorph(in source, in _precondition, ref result, ref postCondition);
    }
}

