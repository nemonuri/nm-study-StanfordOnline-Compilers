namespace DscTool.Scoped.Graphs.Functors;

public delegate bool StrongestConditionLowLevelFactory<T, TCondition>(scoped ref readonly T value, [NotNullWhen(true)] scoped ref TCondition? condition);