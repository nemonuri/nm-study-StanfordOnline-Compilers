namespace DscTool;

public static class DscComponentTheory
{
    extension<
        TResource, TResourceVerificationCondition, 
        TState, TStateVerificationCondition,
        TResponse, TResponseVerificationCondition,
        TDscComponent>
    (ReadOnlyTypeBox<(
        TResource, TResourceVerificationCondition, 
        TState, TStateVerificationCondition,
        TResponse, TResponseVerificationCondition
        ), TDscComponent> theory)
        where TResource : IReadOnlySpanNode<TResource>
        where TState : IState<TState>
        where TResponse : IReadOnlySpanNode<TResponse>
        where TDscComponent : 
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
    {
        public static ref readonly ReadOnlyTypeBox<(
        TResource, TResourceVerificationCondition, 
        TState, TStateVerificationCondition,
        TResponse, TResponseVerificationCondition
        ), TDscComponent>
        Theorize(ref readonly TDscComponent source) =>
            ref TypeBox.ReadOnlyBox<(
            TResource, TResourceVerificationCondition, 
            TState, TStateVerificationCondition,
            TResponse, TResponseVerificationCondition
            ), TDscComponent>(in source);
        
        public TState GetDesiredState(scoped ref readonly TResource resource, [NotNullWhen(true)] out TStateVerificationCondition postCondition)
        {
            DscCommandKindPair<TResource> v = new (DscStateKind.Desired, resource);
            return theory.Self.Morph(in v, out postCondition);
        }

        public TState GetCurrentState(scoped ref readonly TResource resource, [NotNullWhen(true)] out TStateVerificationCondition postCondition)
        {
            DscCommandKindPair<TResource> v = new (DscStateKind.Current, resource);
            return theory.Self.Morph(in v, out postCondition);
        }

        public bool TestState(scoped ref readonly TState snapShot, scoped ref readonly TState desired)
        {
            return theory.Self.IsSubset(in snapShot, in desired);
        }

        public TState EditResource(scoped ref readonly TResource resource, [NotNullWhen(true)] out TStatePredicate postCondition)
        {
            DscCommandKindPair<TResource> v = new (DscStateKind.EditResource, resource);
            return theory.Self.Morph(in v, out postCondition);
        }


    }
}

