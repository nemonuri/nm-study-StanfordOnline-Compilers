using System.Diagnostics;
using S = DscTool.Scoped.ScopedCommandCategoryTheory;

namespace DscTool.Scoped;

public class LiftedScopedHoareTripleCommand<
    TSource, TSourceCondition, TSourceCategory, TSourceCategoriedCommand,
    TTarget, TTargetCondition, TTargetCategory,
    TConditionCategory, TLifter, TEmbedder> :
    IScopedCategoriedCommand<TTarget, TTargetCondition, TTargetCategory>
    where TSourceCategory : IScopedCategory<TSource, TSourceCondition>
    where TSourceCategoriedCommand : IScopedCategoriedCommand<TSource, TSourceCondition, TSourceCategory>
    where TTargetCategory : IScopedCategory<TTarget, TTargetCondition>
    where TConditionCategory : IScopedConditionCategory<TSourceCondition, TTarget, TTargetCondition, TTargetCategory>
    where TLifter : IScopedHoareTripleMorphism<TSource, TSourceCondition, TTarget, TTargetCondition>
    where TEmbedder : IScopedHoareTripleMorphism<TTarget, TTargetCondition, TSource, TSourceCondition>
{
    private readonly TTargetCondition _targetCondition;
    private readonly TSourceCategoriedCommand _sourceCategoriedCommand;
    private readonly TConditionCategory _conditionCategory;
    private readonly TTargetCategory _targetCategory;
    private readonly TLifter _lifter;
    private readonly TEmbedder _embedder;

    public LiftedScopedHoareTripleCommand
    (
        scoped ref readonly TTargetCondition targetCondition,
        scoped ref readonly TSourceCategoriedCommand sourceCategoriedCommand,
        scoped ref readonly TConditionCategory conditionCategory,
        scoped ref readonly TLifter lifter,
        scoped ref readonly TEmbedder embedder
    )
    {
        Debug.Assert(UnsafeReadOnly.IsNotNullRef(in sourceCategoriedCommand));
        Debug.Assert(UnsafeReadOnly.IsNotNullRef(in targetCondition));
        Debug.Assert(UnsafeReadOnly.IsNotNullRef(in conditionCategory));
        Debug.Assert(UnsafeReadOnly.IsNotNullRef(in lifter));
        Debug.Assert(UnsafeReadOnly.IsNotNullRef(in embedder));

        _sourceCategoriedCommand = sourceCategoriedCommand;
        _targetCondition = targetCondition;
        _conditionCategory = conditionCategory;
        _targetCategory = conditionCategory.TargetCategory;
        _lifter = lifter;
        _embedder = embedder;
    }

    public ref readonly TTargetCategory Category => ref _targetCategory;

    public ref readonly TTargetCondition PreCondition => ref _targetCondition;

    public bool TryInvoke
    (
        scoped ref readonly TTarget liftedSource, 
        [NotNullWhen(true)] scoped ref TTarget? liftedResult, 
        [NotNullWhen(true)] scoped ref TTargetCondition? postLiftedCondition
    )
    {
        if (!_conditionCategory.IsSufficient(in _embedder.PreCondition, in _targetCondition)) {return false;}
        TSource? source = default;
        {
            TSourceCondition? embedderPostCondition = default;
            if (!_embedder.TryMorph(in liftedSource, ref source, ref embedderPostCondition)) {return false;}
            if (!_sourceCategoriedCommand.Category.Satisfies(in source, in embedderPostCondition)) {return false;}
        }

        TSource? result = default;
        TSourceCondition? postCondition = default;
        {
            var theory = S.TheorizeCommand<TSource, TSourceCondition, TSourceCategory, TSourceCategoriedCommand>(in _sourceCategoriedCommand);
            if (!theory.TryInvoke(in source, ref result, ref postCondition, out var errorKind)) {return false;}
        }
        
        if (!_sourceCategoriedCommand.Category.Satisfies(in result, in _lifter.PreCondition)) {return false;}
        if (!_lifter.TryMorph(in result, ref liftedResult, ref postLiftedCondition)) {return false;}
        
        return true;
    }
}

