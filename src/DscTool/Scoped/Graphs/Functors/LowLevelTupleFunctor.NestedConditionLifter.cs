
namespace DscTool.Scoped.Graphs.Functors;

public readonly partial struct LowLevelTupleFunctor<
    TSource, TSourceCondition, TSourceCategory, 
    THead, THeadCondition, THeadCategory> 
    where TSourceCategory : IScopedCategory<TSource, TSourceCondition>
    where THeadCategory : IScopedCategory<THead, THeadCondition>
{
    public readonly struct NestedConditionLifter : 
        IScopedHoareTripleMorphism<TSourceCondition, (THeadCondition, TSourceCondition), (THeadCondition, TSourceCondition), (THeadCondition, TSourceCondition)>
    {
        private readonly (THeadCondition, TSourceCondition) _precondition;

        public NestedConditionLifter(TSourceCondition weakestSourceCondition, THeadCondition weakestHeadCondition)
        {
            _precondition = (weakestHeadCondition, weakestSourceCondition);
        }

        [UnscopedRef] public ref readonly (THeadCondition, TSourceCondition) PreCondition => ref _precondition;

        public bool TryMorph
        (
            scoped ref readonly TSourceCondition source, 
            [NotNullWhen(true)] scoped ref (THeadCondition, TSourceCondition) result, 
            [NotNullWhen(true)] scoped ref (THeadCondition, TSourceCondition) postCondition
        )
        {
            result = (_precondition.Item1, source);
            postCondition = _precondition;
            return true;
        }        
    }
}

