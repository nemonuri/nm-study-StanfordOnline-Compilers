namespace DscTool;

public interface IState<TSource>
{
    ref readonly TSource Source {get;}
}
