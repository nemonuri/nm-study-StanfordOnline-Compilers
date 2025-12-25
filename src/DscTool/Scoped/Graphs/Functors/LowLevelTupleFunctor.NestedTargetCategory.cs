
namespace DscTool.Scoped.Graphs.Functors;

public readonly partial struct LowLevelTupleFunctor<
    TSource, TSourceCondition, TSourceCategory, 
    THead, THeadCondition, THeadCategory> 
    where TSourceCategory : IScopedCategory<TSource, TSourceCondition>
    where THeadCategory : IScopedCategory<THead, THeadCondition>
{
    public readonly struct NestedTargetCategory : IScopedCategory<(THead, TSource), (THeadCondition, TSourceCondition)>
    {
        internal readonly TSourceCategory _sourceCategory;
        private readonly THeadCategory _headCategory;

        public NestedTargetCategory(TSourceCategory sourceCategory, THeadCategory headCategory)
        {
            _sourceCategory = sourceCategory;
            _headCategory = headCategory;
        }

        public bool Equals((THead, TSource) x, (THead, TSource) y)
        {
            return
                _headCategory.Equals(x.Item1, y.Item1) &&
                _sourceCategory.Equals(x.Item2, y.Item2);
        }

        public int GetHashCode((THead, TSource) x)
        {
            HashCode hc = default;
            hc.Add(_headCategory.GetHashCode(x.Item1));
            hc.Add(_sourceCategory.GetHashCode(x.Item2));
            return hc.ToHashCode();
        }

        public bool Satisfies
        (
            scoped ref readonly (THead, TSource) value, 
            scoped ref readonly (THeadCondition, TSourceCondition) condition
        )
        {
            return 
                _headCategory.Satisfies(in value.Item1, in condition.Item1) &&
                _sourceCategory.Satisfies(in value.Item2, in condition.Item2);
        }

        public bool IsSufficient
        (
            scoped ref readonly (THeadCondition, TSourceCondition) sufficient, 
            scoped ref readonly (THeadCondition, TSourceCondition) necessary
        )
        {
            return 
                _headCategory.IsSufficient(in sufficient.Item1, in necessary.Item1) &&
                _sourceCategory.IsSufficient(in sufficient.Item2, in necessary.Item2);
        }
    }
}

