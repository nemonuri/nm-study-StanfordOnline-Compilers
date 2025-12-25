using DscTool.Infrastructure;

namespace DscTool.Scoped.Graphs;

public interface IScopedPackedDigraphConditionLifter<TCondition, TNodeKey, TEdgeLabel> :
    IScopedHoareTripleMorphism<PackedDigraph<TNodeKey, TEdgeLabel, TCondition>, TCondition, TCondition, TCondition>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{}

public interface IScopedPackedDigraphLifter<T, TCondition, TNodeKey, TEdgeLabel> :
    IScopedHoareTripleMorphism<PackedDigraph<TNodeKey, TEdgeLabel, T>, PackedDigraph<TNodeKey, TEdgeLabel, TCondition>, T, TCondition>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{}

public interface IScopedPackedDigraphEmbedder<T, TCondition, TNodeKey, TEdgeLabel> :
    IScopedHoareTripleMorphism<T, TCondition, PackedDigraph<TNodeKey, TEdgeLabel, T>, PackedDigraph<TNodeKey, TEdgeLabel, TCondition>>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{}

public interface IScopedPackedDigraphFaithfulFunctor<T, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder, TNodeKey, TEdgeLabel> :
    IScopedFaithfulFunctor<
        PackedDigraph<TNodeKey, TEdgeLabel, T>, PackedDigraph<TNodeKey, TEdgeLabel, TCondition>, T, TCondition, TCategory,
        TConditionLifter, TLifter, TEmbedder>
    where TCategory : IScopedCategory<T, TCondition>
    where TConditionLifter : IScopedPackedDigraphConditionLifter<TCondition, TNodeKey, TEdgeLabel>
    where TLifter : IScopedPackedDigraphLifter<T, TCondition, TNodeKey, TEdgeLabel>
    where TEmbedder : IScopedPackedDigraphEmbedder<T, TCondition, TNodeKey, TEdgeLabel>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{}



public interface IScopedPackedDigraphConditionLifter<TAtomicCondition, TCondition, TNodeKey, TEdgeLabel> :
    IScopedHoareTripleMorphism<PackedDigraph<TNodeKey, TEdgeLabel, TAtomicCondition>, TCondition, TCondition, TCondition>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{}

public interface IScopedPackedDigraphLifter<TAtom, T, TAtomicCondition, TCondition, TNodeKey, TEdgeLabel> :
    IScopedHoareTripleMorphism<PackedDigraph<TNodeKey, TEdgeLabel, TAtom>, PackedDigraph<TNodeKey, TEdgeLabel, TAtomicCondition>, T, TCondition>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{}

public interface IScopedPackedDigraphEmbedder<TAtom, T, TAtomicCondition, TCondition, TNodeKey, TEdgeLabel> :
    IScopedHoareTripleMorphism<T, TCondition, PackedDigraph<TNodeKey, TEdgeLabel, TAtom>, PackedDigraph<TNodeKey, TEdgeLabel, TAtomicCondition>>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{}

public interface IScopedPackedDigraphFaithfulFunctor<TAtom, T, TAtomicCondition, TCondition, TCategory, TConditionLifter, TLifter, TEmbedder, TNodeKey, TEdgeLabel> :
    IScopedFaithfulFunctor<
        PackedDigraph<TNodeKey, TEdgeLabel, TAtom>, PackedDigraph<TNodeKey, TEdgeLabel, TAtomicCondition>, T, TCondition, TCategory,
        TConditionLifter, TLifter, TEmbedder>
    where TCategory : IScopedCategory<T, TCondition>
    where TConditionLifter : IScopedPackedDigraphConditionLifter<TAtomicCondition, TCondition, TNodeKey, TEdgeLabel>
    where TLifter : IScopedPackedDigraphLifter<TAtom, T, TAtomicCondition, TCondition, TNodeKey, TEdgeLabel>
    where TEmbedder : IScopedPackedDigraphEmbedder<TAtom, T, TAtomicCondition, TCondition, TNodeKey, TEdgeLabel>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{}
