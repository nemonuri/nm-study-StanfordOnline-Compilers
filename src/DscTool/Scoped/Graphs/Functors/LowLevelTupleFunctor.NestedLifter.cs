
namespace DscTool.Scoped.Graphs.Functors;

public readonly partial struct LowLevelTupleFunctor<
    TSource, TSourceCondition, TSourceCategory, 
    THead, THeadCondition, THeadCategory> where TSourceCategory : IScopedCategory<TSource, TSourceCondition>
    where THeadCategory : IScopedCategory<THead, THeadCondition>
{
    public readonly struct NestedLifter :
        IScopedHoareTripleMorphism<TSource, TSourceCondition, (THead, TSource), (THeadCondition, TSourceCondition)>
    {
        private readonly TSourceCondition _weakestSourceCondition;
        private readonly StrongestConditionLowLevelFactory<TSource, TSourceCondition> _strongestSourceConditionFactory;
        private readonly THead _defaultHead;
        private readonly THeadCondition _defaultHeadCondition;

        public NestedLifter
        (
            TSourceCondition weakestSourceCondition,
            StrongestConditionLowLevelFactory<TSource, TSourceCondition> strongestSourceConditionFactory,
            THead defaultHead,
            THeadCondition defaultHeadCondition
        )
        {
            _weakestSourceCondition = weakestSourceCondition;
            _strongestSourceConditionFactory = strongestSourceConditionFactory;
            _defaultHead = defaultHead;
            _defaultHeadCondition = defaultHeadCondition;
        }

        [UnscopedRef] public ref readonly TSourceCondition PreCondition => ref _weakestSourceCondition;

        public bool TryMorph
        (
            scoped ref readonly TSource source, 
            [NotNullWhen(true)] scoped ref (THead, TSource) result, 
            [NotNullWhen(true)] scoped ref (THeadCondition, TSourceCondition) postCondition
        )
        {
            TSourceCondition? sourcePostCondition = default;
            if (!_strongestSourceConditionFactory(in source, ref sourcePostCondition)) {return false;}

            result = (_defaultHead, source);
            postCondition = (_defaultHeadCondition, sourcePostCondition);
            return true;
        }
    }
}
