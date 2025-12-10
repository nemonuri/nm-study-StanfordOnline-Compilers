namespace DscTool;

public interface IDesiredResource<TResource, TState>
{
    public TState? DesriedState {get;}
}
