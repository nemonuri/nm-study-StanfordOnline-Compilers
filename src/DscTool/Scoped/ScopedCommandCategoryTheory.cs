namespace DscTool.Scoped;

public static class ScopedCommandCategoryTheory
{
    public static bool CheckCondition<T, TCondition, TConditionChecker>
    (
        scoped ref readonly TConditionChecker checker,
        scoped ref readonly T value,
        scoped ref readonly TCondition condition
    )
        where TConditionChecker : IScopedConditionChecker<T, TCondition>
    {
        return checker.Satisfies(in value, in condition);
    }

    public static bool TryInvoke<T, TCondition, TConditionChecker, TCommand>
    (
        scoped ref readonly TCommand command,
        scoped ref readonly TConditionChecker checker,
        scoped ref readonly T value,
        [NotNullWhen(true)] scoped ref T? result,
        [NotNullWhen(true)] scoped ref TCondition? postCondition,
        out HoareTripleErrorKind errorKind,
        bool skipPreConditionCheck = false
    )
        where TCommand : IScopedHoareTripleCommand<T, TCondition>
        where TConditionChecker : IScopedConditionChecker<T, TCondition>
    {
        if (!skipPreConditionCheck && !CheckCondition(in checker, in value, in command.PreCondition))
        {
            (result, postCondition, errorKind) = (default, default, HoareTripleErrorKind.PreCondition);
            return false;
        }
        if (!command.TryInvoke(in value, ref result, ref postCondition))
        {
            (result, postCondition, errorKind) = (default, default, HoareTripleErrorKind.Invoke);
            return false;
        }
        if (!checker.Satisfies(in result, in postCondition))
        {
            errorKind = HoareTripleErrorKind.PostCondition;
            return false;
        }
        errorKind = HoareTripleErrorKind.Unknown;
        return true;
    }

    public static bool TryMorph<TSource, TSourceCondition, TConditionChecker, TTarget, TTargetCondition, TMorphism>
    (
        scoped ref readonly TMorphism morphism,
        scoped ref readonly TConditionChecker checker,
        scoped ref readonly TSource value,
        [NotNullWhen(true)] scoped ref TTarget? result,
        [NotNullWhen(true)] scoped ref TTargetCondition? postCondition,
        out HoareTripleErrorKind errorKind,
        bool skipPreConditionCheck = false
    )
        where TMorphism : IScopedHoareTripleMorphism<TSource, TSourceCondition, TTarget, TTargetCondition>
        where TConditionChecker : IScopedConditionChecker<TSource, TSourceCondition>
    {
        if (!skipPreConditionCheck && !CheckCondition(in checker, in value, in morphism.PreCondition))
        {
            (result, postCondition, errorKind) = (default, default, HoareTripleErrorKind.PreCondition);
            return false;
        }
        if (!morphism.TryMorph(in value, ref result, ref postCondition))
        {
            (result, postCondition, errorKind) = (default, default, HoareTripleErrorKind.Invoke);
            return false;
        }
        if (!checker.Satisfies(in value, in morphism.PreCondition))
        {
            errorKind = HoareTripleErrorKind.PostCondition;
            return false;
        }
        errorKind = HoareTripleErrorKind.Unknown;
        return true;
    }

    extension<T, TCondition, TCategory, TCategoriedCommand>
    (scoped ref readonly ReadOnlyTypeBox<(T, TCondition, TCategory), TCategoriedCommand> theory)
        where TCategory : IScopedCategory<T, TCondition>
        where TCategoriedCommand : IScopedCategoriedCommand<T, TCondition, TCategory>
    {
        public static ref readonly ReadOnlyTypeBox<(T, TCondition, TCategory), TCategoriedCommand>
        TheorizeCommand(ref readonly TCategoriedCommand source) =>
            ref TypeBox.ReadOnlyBox<(T, TCondition, TCategory), TCategoriedCommand>(in source);
        
        //public bool CheckPrecondtion(scoped ref readonly T value) => CheckCondition(in theory.Self.Category, in value, in theory.Self.PreCondition);
        
        public bool TryInvoke
        (
            scoped ref readonly T value, 
            [NotNullWhen(true)] scoped ref T? result,
            [NotNullWhen(true)] scoped ref TCondition? postCondition,
            out HoareTripleErrorKind errorKind,
            bool skipPreConditionCheck = false
        ) =>
        TryInvoke(in theory.Self, in theory.Self.Category, in value, ref result, ref postCondition, out errorKind, skipPreConditionCheck);
    }

    extension<
        TSource, TSourceCondition, TSourceCategory,
        TTarget, TTargetCondition, TTargetCategory,
        TCategoriedMorphism>
    (scoped ref readonly ReadOnlyTypeBox<(
        TSource, TSourceCondition, TSourceCategory,
        TTarget, TTargetCondition, TTargetCategory
        ), TCategoriedMorphism> theory)
        where TSourceCategory : IScopedCategory<TSource, TSourceCondition>
        where TTargetCategory : IScopedCategory<TTarget, TTargetCondition>
        where TCategoriedMorphism : IScopedCategoriedMorphism<TSource, TSourceCondition, TSourceCategory, TTarget, TTargetCondition, TTargetCategory>
    {
        public static ref readonly ReadOnlyTypeBox<(
        TSource, TSourceCondition, TSourceCategory,
        TTarget, TTargetCondition, TTargetCategory
        ), TCategoriedMorphism>
        TheorizeMorphism(ref readonly TCategoriedMorphism source) =>
            ref TypeBox.ReadOnlyBox<(
        TSource, TSourceCondition, TSourceCategory,
        TTarget, TTargetCondition, TTargetCategory
        ), TCategoriedMorphism>
        (in source);

        //public bool CheckPrecondtion(scoped ref readonly TSource source) => theory.Self.SourceCategory.Satisfies(in source, in theory.Self.PreCondition);
        
        public bool TryMorph
        (
            scoped ref readonly TSource value, 
            [NotNullWhen(true)] scoped ref TTarget? result,
            scoped ref TTargetCondition? postCondition,
            out HoareTripleErrorKind errorKind,
            bool skipPreConditionCheck = false
        ) =>
        TryMorph<TSource, TSourceCondition, TSourceCategory, TTarget, TTargetCondition, TCategoriedMorphism>
            (in theory.Self, in theory.Self.SourceCategory, in value, ref result, ref postCondition, out errorKind, skipPreConditionCheck);
        /*
        {
            if (!skipPreConditionCheck && !theory.CheckPrecondtion(in value))
            {
                (result, postCondition, errorKind) = (default, default, HoareTripleErrorKind.PreCondition);
                return false;
            }
            if (!theory.Self.TryMorph(in value, ref result, ref postCondition))
            {
                (result, postCondition, errorKind) = (default, default, HoareTripleErrorKind.Invoke);
                return false;
            }
            if (!theory.Self.TargetCategory.Satisfies(in result, in postCondition))
            {
                errorKind = HoareTripleErrorKind.PostCondition;
                return false;
            }
            errorKind = HoareTripleErrorKind.Unknown;
            return true;
        }
        */
    }

    extension<
        TSource, TSourceCondition, TSourceCategory,
        TTarget, TTargetCondition, TTargetCategory,
        TConditionCategory>
    (scoped ref readonly ReadOnlyTypeBox<(
        TSource, TSourceCondition, TSourceCategory,
        TTarget, TTargetCondition, TTargetCategory
        ), TConditionCategory> theory)
        where TSourceCategory : IScopedCategory<TSource, TSourceCondition>
        where TTargetCategory : IScopedCategory<TTarget, TTargetCondition>
        where TConditionCategory : IScopedConditionCategory<TSourceCondition, TTarget, TTargetCondition, TTargetCategory>
    {
        public static ref readonly ReadOnlyTypeBox<(
        TSource, TSourceCondition, TSourceCategory,
        TTarget, TTargetCondition, TTargetCategory
        ), TConditionCategory>
        TheorizeCondition(ref readonly TConditionCategory source) =>
            ref TypeBox.ReadOnlyBox<(
        TSource, TSourceCondition, TSourceCategory,
        TTarget, TTargetCondition, TTargetCategory
        ), TConditionCategory>
        (in source);

        public bool TryLift<TConditionLifter, TLifter, TEmbedder, TSourceCategoriedCommand>
        (
            scoped ref readonly TConditionLifter conditionLifter,
            scoped ref readonly TLifter lifter,
            scoped ref readonly TEmbedder embedder,
            scoped ref readonly TSourceCategoriedCommand sourceCommand,
            [NotNullWhen(true)] out IScopedCategoriedCommand<TTarget, TTargetCondition, TTargetCategory>? result,
            out LiftErrorInfo error
        )
            where TConditionLifter : IScopedHoareTripleMorphism<TSourceCondition, TTargetCondition, TTargetCondition, TTargetCondition>
            where TLifter : IScopedHoareTripleMorphism<TSource, TSourceCondition, TTarget, TTargetCondition>
            where TEmbedder : IScopedHoareTripleMorphism<TTarget, TTargetCondition, TSource, TSourceCondition>
            where TSourceCategoriedCommand : IScopedCategoriedCommand<TSource, TSourceCondition, TSourceCategory>
        {
            TTargetCondition? liftedPrecondition = default;
            {
                TTargetCondition? postConditionOfLiftedPrecondition = default;
                if 
                (
                    !TryMorph<TSourceCondition, TTargetCondition, TConditionCategory, TTargetCondition, TTargetCondition, TConditionLifter>
                        (in conditionLifter, in theory.Self, in sourceCommand.PreCondition, ref liftedPrecondition, ref postConditionOfLiftedPrecondition, out HoareTripleErrorKind innerErrorKind)
                )
                {
                    (result, error) = (default, new(LiftErrorKind.ConditionLifterInnerError, innerErrorKind));
                    return false;
                }
                if (!theory.Self.IsSufficient(in liftedPrecondition, in postConditionOfLiftedPrecondition))
                {
                    (result, error) = (default, new(LiftErrorKind.LiftedPreconditionIsNotSufficient, default));
                    return false;
                }
            }
            
            if (!theory.Self.IsSufficient(in embedder.PreCondition, in liftedPrecondition))
            {
                (result, error) = (default, new(LiftErrorKind.EmbedderPreconditionIsNotSufficient, default));
                return false;
            }

            result = new LiftedScopedHoareTripleCommand<
                TSource, TSourceCondition, TSourceCategory, TSourceCategoriedCommand,
                TTarget, TTargetCondition, TTargetCategory,
                TConditionCategory, TLifter, TEmbedder>
                (in liftedPrecondition, in sourceCommand, in theory.Self, in lifter, in embedder);
            error = default;
            return true;
        }
    }
}
