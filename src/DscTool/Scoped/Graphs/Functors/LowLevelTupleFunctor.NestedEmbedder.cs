
namespace DscTool.Scoped.Graphs.Functors;

public readonly partial struct LowLevelTupleFunctor<
    TSource, TSourceCondition, TSourceCategory, 
    THead, THeadCondition, THeadCategory> where TSourceCategory : IScopedCategory<TSource, TSourceCondition>
    where THeadCategory : IScopedCategory<THead, THeadCondition>
{
    public readonly struct NestedEmbedder :
        IScopedHoareTripleMorphism<(THead, TSource), (THeadCondition, TSourceCondition), TSource, TSourceCondition>
    {
        private readonly (THeadCondition, TSourceCondition) _precondition;
        private readonly StrongestConditionLowLevelFactory<TSource, TSourceCondition> _strongestSourceConditionFactory;

        public NestedEmbedder
        (
            TSourceCondition weakestSourceCondition, 
            THeadCondition weakestHeadCondition,
            StrongestConditionLowLevelFactory<TSource, TSourceCondition> strongestSourceConditionFactory
        )
        {
            _precondition = (weakestHeadCondition, weakestSourceCondition);
            _strongestSourceConditionFactory = strongestSourceConditionFactory;
        }

        [UnscopedRef] public ref readonly (THeadCondition, TSourceCondition) PreCondition => ref _precondition;

        public bool TryMorph
        (
            scoped ref readonly (THead, TSource) source, 
            [NotNullWhen(true)] scoped ref TSource? result, 
            [NotNullWhen(true)] scoped ref TSourceCondition? postCondition
        )
        {
            TSourceCondition? sourcePostCondition = default;
            if (!_strongestSourceConditionFactory(in source.Item2, ref sourcePostCondition)) {return false;}

            result = source.Item2!;
            postCondition = sourcePostCondition;
            return true;
        }

        
    }
}

