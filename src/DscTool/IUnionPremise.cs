namespace DscTool;

public interface IUnionPremise<TUnion>
{
    bool CanLift<T>();
    ref readonly TUnion Lift<T>(scoped ref readonly T t);
    bool TryEmbed<T>(scoped ref readonly TUnion? union, [NotNullWhen(true)]out T? embedded);
}
