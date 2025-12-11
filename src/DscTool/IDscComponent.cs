namespace DscTool;

public interface IDscComponent<
    TResource, TResourceVerificationCondition, 
    TState, TStateVerificationCondition,
    TResponse, TResponseVerificationCondition> : 
    IHoareTripleMorphism<
        DscCommandKindPair<TResource>, 
        DscCommandKindMap<TResourceVerificationCondition>, 
        TState, 
        TStateVerificationCondition>,
    IHoareTripleMorphism<
        DesiredSnapshotPair<TState>,
        TStateVerificationCondition,
        DscCommandKindPair<TResponse>,
        DscCommandKindMap<TResponseVerificationCondition>>
    where TResource : IReadOnlySpanNode<TResource>
    where TState : IState<TState>
    where TResponse : IReadOnlySpanNode<TResponse>
{
}

public interface IReadOnlySpanNode<TNode> 
    where TNode : IReadOnlySpanNode<TNode>
{
    ReadOnlySpan<TNode> GetChildrenAsReadOnlySpan ();
}

public interface IState<TState> : IReadOnlySpanNode<TState>
    where TState : IState<TState>
{
    bool Exist {get;}
}

public interface IResponse<TResponse> : IReadOnlySpanNode<TResponse>
    where TResponse : IState<TResponse>
{
    bool InDesiredState {get;}
}

public readonly record struct DesiredSnapshotPair<T>(T Desried, T Snapshot);

public readonly record struct DscCommandKindPair<T>(DscStateKind Key, T Value);

public readonly record struct DscCommandKindMap<T>
(
    T CellOfGetDesiredState,
    T CellOfGetStateSnapshot,
    T CellOfEditResource
)
{
    public bool TryGetCell(DscStateKind kind, [NotNullWhen(true)] out T? cell) => 
    ((_, cell) = kind switch
    {
        DscStateKind.Desired => (true, CellOfGetDesiredState),
        DscStateKind.Current => (true, CellOfGetStateSnapshot),
        DscStateKind.EditResource => (true, CellOfEditResource),
        _ => (false, default)
    }
    ) is (true, _);

    public T this[DscStateKind kind] => kind switch
    {
        DscStateKind.Desired => CellOfGetDesiredState,
        DscStateKind.Current => CellOfGetStateSnapshot,
        DscStateKind.EditResource => CellOfEditResource,
        _ => ThrowHelper.ThrowArgumentOutOfRangeException<T>(nameof(kind), kind, default)
    };
}
