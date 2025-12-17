
namespace DscTool.Scoped.Sequences;

public interface IScopedMemoryConditionLifter<TCondition> :
    IScopedHoareTripleMorphism<Memory<TCondition>, TCondition, TCondition, TCondition>
{}

public interface IScopedMemoryLifter<T, TCondition> :
    IScopedHoareTripleMorphism<Memory<T>, Memory<TCondition>, T, TCondition>
{}

public interface IScopedMemoryEmbedder<T, TCondition> :
    IScopedHoareTripleMorphism<T, TCondition, Memory<T>, Memory<TCondition>>
{}

public interface IScopedMemoryFaithfulFunctor<T, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder> :
    IScopedFaithfulFunctor<
        Memory<T>, Memory<TCondition>, T, TCondition, TCategory,
        TConditionLifter, TLifter, TEmbedder>
    where TCategory : IScopedCategory<T, TCondition>
    where TConditionLifter : IScopedMemoryConditionLifter<TCondition>
    where TLifter : IScopedMemoryLifter<T, TCondition>
    where TEmbedder : IScopedMemoryEmbedder<T, TCondition>
{}



public interface IScopedMemoryConditionLifter<TAtomicCondition, TCondition> :
    IScopedHoareTripleMorphism<Memory<TAtomicCondition>, TCondition, TCondition, TCondition>
{}

public interface IScopedMemoryLifter<TAtom, T, TAtomicCondition, TCondition> :
    IScopedHoareTripleMorphism<Memory<TAtom>, Memory<TAtomicCondition>, T, TCondition>
{}

public interface IScopedMemoryEmbedder<TAtom, T, TAtomicCondition, TCondition> :
    IScopedHoareTripleMorphism<T, TCondition, Memory<TAtom>, Memory<TAtomicCondition>>
{}

public interface IScopedMemoryFaithfulFunctor<TAtom, T, TAtomicCondition, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder> :
    IScopedFaithfulFunctor<
        Memory<TAtom>, Memory<TAtomicCondition>, T, TCondition, TCategory,
        TConditionLifter, TLifter, TEmbedder>
    where TCategory : IScopedCategory<T, TCondition>
    where TConditionLifter : IScopedMemoryConditionLifter<TAtomicCondition, TCondition>
    where TLifter : IScopedMemoryLifter<TAtom, T, TAtomicCondition, TCondition>
    where TEmbedder : IScopedMemoryEmbedder<TAtom, T, TAtomicCondition, TCondition>
{}
