namespace DscTool;

public delegate ref readonly TUnion Lifter<TUnion, T>(scoped ref readonly T t);

public interface ILiftable<TUnion, TSelf>
    where TSelf : ILiftable<TUnion, TSelf>
{
    Lifter<TUnion, TSelf>? FallBackLifter {get;}
}

public interface ISubUnionPremise<TUnion, TUnionPremise>
    where TUnionPremise : IUnionPremise<TUnion>
{
    /* Just type assertion */
}
