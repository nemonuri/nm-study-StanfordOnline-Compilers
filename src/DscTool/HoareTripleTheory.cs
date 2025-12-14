
using System.Diagnostics;

namespace DscTool;

public static class HoareTripleTheory
{
    extension<T, TPredicate, TCommand>
    (scoped ref readonly ReadOnlyTypeBox<(T, TPredicate), TCommand> commandTheory)
        where TCommand : IHoareTripleCommand<T, TPredicate>
    {
        public static ref readonly ReadOnlyTypeBox<(T, TPredicate), TCommand>
        TheorizeCommand(ref readonly TCommand source) =>
            ref TypeBox.ReadOnlyBox<(T, TPredicate), TCommand>(in source);
        
        public bool CheckInvokePreCondition<TPredicatePremise>(scoped ref readonly TPredicatePremise predicatePremise, scoped ref readonly T value)
            where TPredicatePremise : IConditionChecker<T, TPredicate>
        {
            return predicatePremise.Satisfies(in value, in commandTheory.Self.PreCondition);
        }

        public Result<T, HoareTripleErrorKind> InvokeAndBrief<TPredicatePremise>
        (
            scoped ref readonly TPredicatePremise prePostConditionChecker, 
            scoped ref readonly T source,
            out TPredicate? postCondition,
            bool skipPreConditionCheck = false
        )
            where TPredicatePremise : IConditionChecker<T, TPredicate>
        {
            if (!skipPreConditionCheck && !commandTheory.CheckInvokePreCondition(in prePostConditionChecker, in source)) 
            {
                postCondition = default;
                return ResultTagger.Error(HoareTripleErrorKind.PreCondition);
            }
            T invokeResult = commandTheory.Self.Invoke(in source, out postCondition);
            if (!prePostConditionChecker.Satisfies(in invokeResult, in postCondition))
            {
                return ResultTagger.Error(HoareTripleErrorKind.PostCondition);
            }
            return ResultTagger.Ok(invokeResult);
        }
    }

    extension<
        TSource, TSourcePredicate, TTarget, TTargetPredicate, 
        TCommand>
    (scoped ref readonly ReadOnlyTypeBox<(TSource, TSourcePredicate, TTarget, TTargetPredicate
        ), TCommand> commandTheory)
        where TCommand : IHoareTripleCommand<TSource, TSourcePredicate>
    {
        public static ref readonly ReadOnlyTypeBox<(
        TSource, TSourcePredicate, TTarget, TTargetPredicate
        ), TCommand>
        TheorizeCommand(ref readonly TCommand source) =>
            ref TypeBox.ReadOnlyBox<(
        TSource, TSourcePredicate, TTarget, TTargetPredicate
        ), TCommand>
        (in source);

        public 
        Result< 
            HoareTripleLiftedCommand<
            TSource, TSourcePredicate, TTarget, TTargetPredicate,
            TCommand, TValueLifter, TPredicateLifter, TValueEmbeder, TPostConditionSemiGroup>,
            HoareTripleLiftCommandErrorKind>
        LiftCommandAndBrief<TPredicateLifter, TPredicateSetChecker, TPredicateSubsetChecker, TValueLifter, TValueEmbeder, TPostConditionSemiGroup>
        (
            scoped ref readonly TPredicateLifter predicateLifter,
            scoped ref readonly TPredicateSetChecker predicatePreconditionChecker,
            scoped ref readonly TPredicateSubsetChecker predicatePostconditionChecker,
            scoped ref readonly TValueLifter valueLifter,
            scoped ref readonly TValueEmbeder valueEmbeder,
            scoped ref readonly TPostConditionSemiGroup postConditionSemiGroup
        )
            where TPredicateLifter : IHoareTripleMorphism<TSourcePredicate, TTargetPredicate, TTargetPredicate, TTargetPredicate>
            where TPredicateSetChecker : IConditionChecker<TSourcePredicate, TTargetPredicate>
            where TPredicateSubsetChecker : IConditionChecker<TTargetPredicate, TTargetPredicate>
            where TValueLifter : IHoareTripleMorphism<TSource, TSourcePredicate, TTarget, TTargetPredicate>
            where TValueEmbeder : IHoareTripleMorphism<TTarget, TTargetPredicate, TSource, TSourcePredicate>
            where TPostConditionSemiGroup : ISemiGroup<TTargetPredicate>
        {
            ref readonly var predicateLiftTheory = ref TheorizeLift<TSourcePredicate, TTargetPredicate, TPredicateLifter>(in predicateLifter);

            Result<TTargetPredicate, HoareTripleErrorKind> predicateLiftResult = 
                predicateLiftTheory.LiftAndBrief(in predicatePreconditionChecker, in predicatePostconditionChecker, in commandTheory.Self.PreCondition, out var predicateLiftPostCondition);
            
            if (predicateLiftResult.IsError)
            {
                return ResultTagger.Error
                (
                    predicateLiftResult.GetError() switch
                    {
                        HoareTripleErrorKind.PreCondition => HoareTripleLiftCommandErrorKind.PreConditionLiftPreCondition,
                        HoareTripleErrorKind.PostCondition => HoareTripleLiftCommandErrorKind.PreConditionLiftPostCondition,
                        _ => HoareTripleLiftCommandErrorKind.Unknown
                    }
                );
            }

            Debug.Assert(predicateLiftResult.IsOk);
            if (predicateLiftPostCondition is null)
            {
                return ResultTagger.Error(HoareTripleLiftCommandErrorKind.PredicateLiftPostConditionIsNull);
            }

            return ResultTagger.Ok<
                HoareTripleLiftedCommand<
                TSource, TSourcePredicate, TTarget, TTargetPredicate,
                TCommand, TValueLifter, TPredicateLifter, TValueEmbeder, TPostConditionSemiGroup>
            >(new (
                predicateLiftPostCondition,
                valueEmbeder,
                commandTheory.Self,
                valueLifter, 
                predicateLifter,
                postConditionSemiGroup
            ));
        }
    }

    extension<
        TSource, TSourcePredicate, TTarget, TTargetPredicate, TMorphism>
    (scoped ref readonly ReadOnlyTypeBox<(
        TSource, TSourcePredicate, TTarget, TTargetPredicate
        ), TMorphism> morphisemTheory)
        where TMorphism : IHoareTripleMorphism<TSource, TSourcePredicate, TTarget, TTargetPredicate>
    {
        public static ref readonly ReadOnlyTypeBox<(
        TSource, TSourcePredicate, TTarget, TTargetPredicate
        ), TMorphism>
        TheorizeMorphism(ref readonly TMorphism source) =>
            ref TypeBox.ReadOnlyBox<(
        TSource, TSourcePredicate, TTarget, TTargetPredicate
        ), TMorphism>
        (in source);

        public bool CheckMorphPreCondition<TSourcePredicatePremise>(scoped ref readonly TSourcePredicatePremise sourcePredicatePremise, scoped ref readonly TSource source)
            where TSourcePredicatePremise : IConditionChecker<TSource, TSourcePredicate>
        {
            return sourcePredicatePremise.Satisfies(in source, in morphisemTheory.Self.PreCondition);
        }

        public Result<TTarget, HoareTripleErrorKind> MorphAndBrief<TSourcePredicatePremise, TTargetPredicatePremise>
        (
            scoped ref readonly TSourcePredicatePremise sourcePredicatePremise, 
            scoped ref readonly TTargetPredicatePremise targetPredicatePremise,
            scoped ref readonly TSource source,
            out TTargetPredicate? postCondition,
            bool skipPreConditionCheck = false
        )
            where TSourcePredicatePremise : IConditionChecker<TSource, TSourcePredicate>
            where TTargetPredicatePremise : IConditionChecker<TTarget, TTargetPredicate>
        {
            if (!skipPreConditionCheck && !morphisemTheory.CheckMorphPreCondition(in sourcePredicatePremise, in source)) 
            {
                postCondition = default;
                return ResultTagger.Error(HoareTripleErrorKind.PreCondition);
            }
            TTarget morphed = morphisemTheory.Self.Morph(in source, out postCondition);
            if (!targetPredicatePremise.Satisfies(in morphed, in postCondition))
            {
                return ResultTagger.Error(HoareTripleErrorKind.PostCondition);
            }
            return ResultTagger.Ok(morphed);
        }
    }

    extension<
        TSourcePredicate, TTargetPredicate, TPredicateMorphism>
    (scoped ref readonly ReadOnlyTypeBox<(
        TSourcePredicate, TTargetPredicate
        ), TPredicateMorphism> liftTheory)
        where TPredicateMorphism : IHoareTripleMorphism<TSourcePredicate, TTargetPredicate, TTargetPredicate, TTargetPredicate>
    {
        public static ref readonly ReadOnlyTypeBox<(
        TSourcePredicate, TTargetPredicate
        ), TPredicateMorphism>
        TheorizeLift(ref readonly TPredicateMorphism source) =>
            ref TypeBox.ReadOnlyBox<(
        TSourcePredicate, TTargetPredicate
        ), TPredicateMorphism>
        (in source);

        public bool CheckLiftPreCondition<TLiftPremise>(scoped ref readonly TLiftPremise liftPremise, scoped ref readonly TSourcePredicate source)
            where TLiftPremise : IConditionChecker<TSourcePredicate, TTargetPredicate>
        {
            return liftPremise.Satisfies(in source, in liftTheory.Self.PreCondition);
        }

        public Result<TTargetPredicate, HoareTripleErrorKind> LiftAndBrief<TLiftPremise, TTargetSubsetPremise>
        (
            scoped ref readonly TLiftPremise liftPremise, 
            scoped ref readonly TTargetSubsetPremise targetSubsetPremise,
            scoped ref readonly TSourcePredicate source,
            out TTargetPredicate? postCondition,
            bool skipPreConditionCheck = false
        )
            where TLiftPremise : IConditionChecker<TSourcePredicate, TTargetPredicate>
            where TTargetSubsetPremise : IConditionChecker<TTargetPredicate, TTargetPredicate>
        {
            if (!skipPreConditionCheck && !liftTheory.CheckLiftPreCondition(in liftPremise, in source)) 
            {
                postCondition = default;
                return ResultTagger.Error(HoareTripleErrorKind.PreCondition);
            }
            TTargetPredicate morphed = liftTheory.Self.Morph(in source, out postCondition);
            if (!targetSubsetPremise.Satisfies(in morphed, in postCondition))
            {
                return ResultTagger.Error(HoareTripleErrorKind.PostCondition);
            }
            return ResultTagger.Ok(morphed);
        }
    }

    extension<
        TSourcePredicate, TTargetPredicate, TPredicateMorphism>
    (scoped ref readonly ReadOnlyTypeBox<(
        TSourcePredicate, TTargetPredicate
        ), TPredicateMorphism> embedTheory)
        where TPredicateMorphism : IHoareTripleMorphism<TSourcePredicate, TSourcePredicate, TTargetPredicate, TSourcePredicate>
    {
        public static ref readonly ReadOnlyTypeBox<(
        TSourcePredicate, TTargetPredicate
        ), TPredicateMorphism>
        TheorizeEmbed(ref readonly TPredicateMorphism source) =>
            ref TypeBox.ReadOnlyBox<(
        TSourcePredicate, TTargetPredicate
        ), TPredicateMorphism>
        (in source);

        public bool CheckEmbedPreCondition<TSourceSubsetPremise>(scoped ref readonly TSourceSubsetPremise sourceSubsetPremise, scoped ref readonly TSourcePredicate source)
            where TSourceSubsetPremise : ISufficientChecker<TSourcePredicate>
        {
            return sourceSubsetPremise.IsSufficient(in source, in embedTheory.Self.PreCondition);
        }

        public Result<TTargetPredicate, HoareTripleErrorKind> EmbedAndBrief<TSourceSubsetPremise, TEmbedPremise>
        (
            scoped ref readonly TSourceSubsetPremise sourceSubsetPremise, 
            scoped ref readonly TEmbedPremise embedPremise,
            scoped ref readonly TSourcePredicate source,
            out TSourcePredicate? postCondition,
            bool skipPreConditionCheck = false
        )
            where TSourceSubsetPremise : ISufficientChecker<TSourcePredicate>
            where TEmbedPremise : IConditionChecker<TTargetPredicate, TSourcePredicate>
        {
            if (!skipPreConditionCheck && !embedTheory.CheckEmbedPreCondition(in sourceSubsetPremise, in source)) 
            {
                postCondition = default;
                return ResultTagger.Error(HoareTripleErrorKind.PreCondition);
            }
            TTargetPredicate morphed = embedTheory.Self.Morph(in source, out postCondition);
            if (!embedPremise.Satisfies(in morphed, in postCondition))
            {
                return ResultTagger.Error(HoareTripleErrorKind.PostCondition);
            }
            return ResultTagger.Ok(morphed);
        }
    }
}
