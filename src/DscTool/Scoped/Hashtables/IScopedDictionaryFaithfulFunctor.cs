using DscTool.Infrastructure;

namespace DscTool.Scoped.Hashtables;

public interface IScopedDictionaryConditionLifter<TCondition, TKey> :
    IScopedHoareTripleMorphism<
        Memory<KeyValuePair<TKey, TCondition>>, ReadOnlyDictionaryFallbackPair<TKey, TCondition>, 
        KeyValuePair<TKey, TCondition>, ReadOnlyDictionaryFallbackPair<TKey, TCondition>>
    where TKey : IEquatable<TKey>
{}

public interface IScopedDictionaryLifter<T, TCondition, TKey> :
    IScopedHoareTripleMorphism<
        Memory<KeyValuePair<TKey, T>>, ReadOnlyDictionaryFallbackPair<TKey, TCondition>, 
        KeyValuePair<TKey, T>, ReadOnlyDictionaryFallbackPair<TKey, TCondition>>
    where TKey : IEquatable<TKey>
{}

public interface IScopedDictionaryEmbedder<T, TCondition, TKey> :
    IScopedHoareTripleMorphism<
        KeyValuePair<TKey, T>, ReadOnlyDictionaryFallbackPair<TKey, TCondition>, 
        Memory<KeyValuePair<TKey, T>>, ReadOnlyDictionaryFallbackPair<TKey, TCondition>>
    where TKey : IEquatable<TKey>
{}

public interface IScopedDictionaryFaithfulFunctor<T, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder, TKey> :
    IScopedFaithfulFunctor<
        Memory<KeyValuePair<TKey, T>>, ReadOnlyDictionaryFallbackPair<TKey, TCondition>, 
        T, TCondition, 
        TCategory,
        TConditionLifter, TLifter, TEmbedder>
    where TCategory : IScopedCategory<T, TCondition>
    where TConditionLifter : IScopedDictionaryConditionLifter<TCondition, TKey>
    where TLifter : IScopedDictionaryLifter<T, TCondition, TKey>
    where TEmbedder : IScopedDictionaryEmbedder<T, TCondition, TKey>
    where TKey : IEquatable<TKey>
{}


public interface IScopedDictionaryConditionLifter<TAtomicCondition, TCondition, TKey> :
    IScopedHoareTripleMorphism<Memory<TAtomicCondition>, TCondition, TCondition, TCondition>
    where TKey : IEquatable<TKey>
{}

public interface IScopedDictionaryLifter<TAtom, T, TAtomicCondition, TCondition, TKey> :
    IScopedHoareTripleMorphism<Memory<TAtom>, Memory<TAtomicCondition>, T, TCondition>
    where TKey : IEquatable<TKey>
{}

public interface IScopedDictionaryEmbedder<TAtom, T, TAtomicCondition, TCondition, TKey> :
    IScopedHoareTripleMorphism<T, TCondition, Memory<TAtom>, Memory<TAtomicCondition>>
    where TKey : IEquatable<TKey>
{}

public interface IScopedDictionaryFaithfulFunctor<TAtom, T, TAtomicCondition, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder, TKey> :
    IScopedFaithfulFunctor<
        Memory<TAtom>, Memory<TAtomicCondition>, T, TCondition, TCategory,
        TConditionLifter, TLifter, TEmbedder>
    where TCategory : IScopedCategory<T, TCondition>
    where TConditionLifter : IScopedDictionaryConditionLifter<TAtomicCondition, TCondition>
    where TLifter : IScopedDictionaryLifter<TAtom, T, TAtomicCondition, TCondition>
    where TEmbedder : IScopedDictionaryEmbedder<TAtom, T, TAtomicCondition, TCondition>
    where TKey : IEquatable<TKey>
{}
