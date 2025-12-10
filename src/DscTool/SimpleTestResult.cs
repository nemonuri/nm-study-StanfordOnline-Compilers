namespace DscTool;

public record class SimpleTestResult<TState>()
{
    public required TState DesiredState {get;init;}
    public required TState ActualState {get;init;}
    public required bool InDesiredState {get;init;}
    public required ImmutableArray<string> DifferingProperties {get;init;}
}
