using DscTool.Infrastructure;

namespace DscTool.Scoped.Hashtables;

public interface IScopedPackedMapConditionLifter<TCondition, TKey> :
    IScopedHoareTripleMorphism<PackedMap<TKey, TCondition>, TCondition, TCondition, TCondition>
    where TKey : IEquatable<TKey>
{}

public interface IScopedPackedMapLifter<T, TCondition, TKey> :
    IScopedHoareTripleMorphism<PackedMap<TKey, T>, PackedMap<TKey, TCondition>, T, TCondition>
    where TKey : IEquatable<TKey>
{}

public interface IScopedPackedMapEmbedder<T, TCondition, TKey> :
    IScopedHoareTripleMorphism<T, TCondition, PackedMap<TKey, T>, PackedMap<TKey, TCondition>>
    where TKey : IEquatable<TKey>
{}

public interface IScopedPackedMapFaithfulFunctor<T, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder, TKey> :
    IScopedFaithfulFunctor<
        PackedMap<TKey, T>, PackedMap<TKey, TCondition>, T, TCondition, TCategory,
        TConditionLifter, TLifter, TEmbedder>
    where TCategory : IScopedCategory<T, TCondition>
    where TConditionLifter : IScopedPackedMapConditionLifter<TCondition, TKey>
    where TLifter : IScopedPackedMapLifter<T, TCondition, TKey>
    where TEmbedder : IScopedPackedMapEmbedder<T, TCondition, TKey>
    where TKey : IEquatable<TKey>
{}



public interface IScopedPackedMapConditionLifter<TAtomicCondition, TCondition, TKey> :
    IScopedHoareTripleMorphism<PackedMap<TKey, TAtomicCondition>, TCondition, TCondition, TCondition>
    where TKey : IEquatable<TKey>
{}

public interface IScopedPackedMapLifter<TAtom, T, TAtomicCondition, TCondition, TKey> :
    IScopedHoareTripleMorphism<PackedMap<TKey, TAtom>, PackedMap<TKey, TAtomicCondition>, T, TCondition>
    where TKey : IEquatable<TKey>
{}

public interface IScopedPackedMapEmbedder<TAtom, T, TAtomicCondition, TCondition, TKey> :
    IScopedHoareTripleMorphism<T, TCondition, PackedMap<TKey, TAtom>, PackedMap<TKey, TAtomicCondition>>
    where TKey : IEquatable<TKey>
{}

public interface IScopedPackedMapFaithfulFunctor<TAtom, T, TAtomicCondition, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder, TKey> :
    IScopedFaithfulFunctor<
        PackedMap<TKey, TAtom>, PackedMap<TKey, TAtomicCondition>, T, TCondition, TCategory,
        TConditionLifter, TLifter, TEmbedder>
    where TCategory : IScopedCategory<T, TCondition>
    where TConditionLifter : IScopedPackedMapConditionLifter<TAtomicCondition, TCondition, TKey>
    where TLifter : IScopedPackedMapLifter<TAtom, T, TAtomicCondition, TCondition, TKey>
    where TEmbedder : IScopedPackedMapEmbedder<TAtom, T, TAtomicCondition, TCondition, TKey>
    where TKey : IEquatable<TKey>
{}
