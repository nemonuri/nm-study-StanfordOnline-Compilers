namespace DscTool;

public delegate ref readonly TUnion Lifter<TUnion, T>(scoped ref readonly T t);

public interface ILiftable<TUnion, TUnionPremise, TSelf>
    where TUnionPremise : IUnionPremise<TUnion>
    where TSelf : ILiftable<TUnion, TUnionPremise, TSelf>
{
    Lifter<TUnion, TSelf>? FallBackLifter {get;}
}