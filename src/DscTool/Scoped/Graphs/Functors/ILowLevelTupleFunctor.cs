
namespace DscTool.Scoped.Graphs.Functors;

public interface ILowLevelTupleFunctor<
    TSource, TSourceCondition, TSourceCategory, 
    THead, THeadCondition, THeadCategory> :
    IScopedFaithfulFunctor<
        TSource, TSourceCondition, (THead, TSource), (THeadCondition, TSourceCondition),
        LowLevelTupleFunctor<
        TSource, TSourceCondition, TSourceCategory, 
        THead, THeadCondition, THeadCategory>.NestedTargetCategory,
        LowLevelTupleFunctor<
        TSource, TSourceCondition, TSourceCategory, 
        THead, THeadCondition, THeadCategory>.NestedConditionLifter,
        LowLevelTupleFunctor<
        TSource, TSourceCondition, TSourceCategory, 
        THead, THeadCondition, THeadCategory>.NestedLifter,
        LowLevelTupleFunctor<
        TSource, TSourceCondition, TSourceCategory, 
        THead, THeadCondition, THeadCategory>.NestedEmbedder
        >
    where TSourceCategory : IScopedCategory<TSource, TSourceCondition>
    where THeadCategory : IScopedCategory<THead, THeadCondition>
{}

