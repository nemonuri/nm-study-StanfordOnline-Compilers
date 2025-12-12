using System.Runtime.InteropServices;

namespace DscTool;

public interface IDscComponent<
    TResource, TResourceVerificationCondition, 
    TState, TStateVerificationCondition,
    TStateSchema, TStateSchemaVerificationCondition,
    TResponse, TResponseVerificationCondition> : 
    IHoareTripleMorphism<
        TagStateSchema<TResource>, 
        TResourceVerificationCondition, 
        TStateSchema, 
        TStateSchemaVerificationCondition>,
    IHoareTripleMorphism<
        TagState<TResource>, 
        TResourceVerificationCondition, 
        TState, 
        TStateVerificationCondition>,
    IHoareTripleMorphism<
        TagTest<StateSchemaPair<TState, TStateSchema>>,
        StateSchemaPair<TStateVerificationCondition, TStateSchemaVerificationCondition>,
        TResponse,
        TResponseVerificationCondition>,
    IHoareTripleMorphism<
        TagEdit<StateSchemaPair<TState, TStateSchema>>,
        StateSchemaPair<TStateVerificationCondition, TStateSchemaVerificationCondition>,
        TResponse,
        TResponseVerificationCondition>
    where TState : IState<TState>
    where TStateSchema : IStateSchema<TState, TStateSchema>
    where TResponse : IResponse<TState, TStateSchema, TResponse>
{
}

public interface IReadOnlySpanNode<TNode> 
    where TNode : IReadOnlySpanNode<TNode>
{
    ReadOnlySpan<TNode> GetChildrenAsReadOnlySpan ();
}

public interface ISupportExist
{
    bool Exist {get;}
}

public interface IState<TState> : 
    IReadOnlySpanNode<TState>, ISupportExist
    where TState : IState<TState>
{
}

public interface IStateSchema<TState, TStateSchema> : 
    ITreeHomomorphismTargetRoot<TState, TStateSchema>, ISupportExist
    where TState : IState<TState>
{
}

public interface ISupportInDesiredState
{
    bool InDesiredState {get;}
}

public interface IResponse<TState, TStateSchema, TResponse> : 
    IReadOnlySpanNode<TResponse>,
    ISupportInDesiredState
    where TState : IState<TState>
    where TStateSchema : IStateSchema<TState, TStateSchema>
    where TResponse : IReadOnlySpanNode<TResponse>
{
    [UnscopedRef] ref readonly TState EquivalentState {get;}
    [UnscopedRef] ref readonly TStateSchema EquivalentStateSchema {get;}
}


[StructLayout(LayoutKind.Sequential)]
public readonly record struct TagStateSchema<T>
{
    public readonly T Self;
    public TagStateSchema(T self) {Self = self;}
}

[StructLayout(LayoutKind.Sequential)]
public readonly record struct TagState<T>
{
    public readonly T Self;
    public TagState(T self) {Self = self;}
}

[StructLayout(LayoutKind.Sequential)]
public readonly record struct StateSchemaPair<TState, TStateSchema>
{
    public readonly TState State;
    public readonly TStateSchema StateSchema;

    public StateSchemaPair(TState state, TStateSchema stateSchema)
    {
        State = state;
        StateSchema = stateSchema;
    }
}

[StructLayout(LayoutKind.Sequential)]
public readonly record struct TagTest<T>
{
    public readonly T Self;
    public TagTest(T self) {Self = self;}
}

[StructLayout(LayoutKind.Sequential)]
public readonly record struct TagEdit<T>
{
    public readonly T Self;
    public TagEdit(T self) {Self = self;}
}

public readonly record struct DscStateKindPaired<T>(DscStateKind Key, T Value);

public readonly record struct DscStateKindMap<T>
(
    T CellOfDesired,
    T CellOfCurrent
);

public readonly record struct DscResponseKindPaired<T>(DscResponseKind Key, T Value);

public readonly record struct DscResponseKindMap<T>
(
    T CellOfTest,
    T CellOfEdit
);
