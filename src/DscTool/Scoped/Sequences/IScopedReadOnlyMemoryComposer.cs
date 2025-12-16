
namespace DscTool.Scoped.Sequences;

public interface IScopedReadOnlyMemoryComposer<TSource, TSourceCondition, TTarget, TTargetCondition> :
    IScopedMorphism<ReadOnlyMemory<TSource>, ReadOnlyMemory<TSourceCondition>, TTarget, TTargetCondition>
{}