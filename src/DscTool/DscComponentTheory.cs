using System.Runtime.CompilerServices;

namespace DscTool;

public static class DscComponentTheory
{
    extension<
        TResource, TResourceVerificationCondition, 
        TState, TStateVerificationCondition,
        TStateSchema, TStateSchemaVerificationCondition,
        TResponse, TResponseVerificationCondition,
        TDscComponent>
    (ReadOnlyTypeBox<(
        TResource, TResourceVerificationCondition, 
        TState, TStateVerificationCondition,
        TStateSchema, TStateSchemaVerificationCondition,
        TResponse, TResponseVerificationCondition
        ), TDscComponent> theory)
        where TResource : IReadOnlySpanNode<TResource>
        where TState : IState<TState>
        where TResponse : IReadOnlySpanNode<TResponse>
        where TDscComponent : 
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
    {
        public static ref readonly ReadOnlyTypeBox<(
        TResource, TResourceVerificationCondition, 
        TState, TStateVerificationCondition,
        TStateSchema, TStateSchemaVerificationCondition,
        TResponse, TResponseVerificationCondition
        ), TDscComponent>
        Theorize(ref readonly TDscComponent source) =>
            ref TypeBox.ReadOnlyBox<(
            TResource, TResourceVerificationCondition, 
            TState, TStateVerificationCondition,
            TStateSchema, TStateSchemaVerificationCondition,
            TResponse, TResponseVerificationCondition
            ), TDscComponent>(in source);
        
        public TStateSchema GetDesiredStateSchema(scoped ref readonly TResource resource, [NotNullWhen(true)] out TStateSchemaVerificationCondition postCondition)
        {
            ref readonly var v = ref UnsafeReadOnly.As<TResource, TagStateSchema<TResource>>(in resource);
            return theory.Self.Morph(in v, out postCondition);
        }

        public TState GetCurrentState(scoped ref readonly TResource resource, [NotNullWhen(true)] out TStateVerificationCondition postCondition)
        {
            ref readonly var v = ref UnsafeReadOnly.As<TResource, TagState<TResource>>(in resource);
            return theory.Self.Morph(in v, out postCondition);
        }

        public TResponse TestState
        (
            scoped ref readonly TState state,
            scoped ref readonly TStateSchema stateSchema, 
            [NotNullWhen(true)] out TResponseVerificationCondition postCondition
        )
        {
            TagTest<StateSchemaPair<TState, TStateSchema>> v = new(new(state, stateSchema));
            return theory.Self.Morph(in v, out postCondition);
        }

        public TResponse EditResource
        (
            scoped ref readonly TState state,
            scoped ref readonly TStateSchema stateSchema, 
            [NotNullWhen(true)] out TResponseVerificationCondition postCondition
        )
        {
            TagEdit<StateSchemaPair<TState, TStateSchema>> v = new(new(state, stateSchema));
            return theory.Self.Morph(in v, out postCondition);
        }
    }
}

