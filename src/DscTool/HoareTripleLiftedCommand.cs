
namespace DscTool;

public class HoareTripleLiftedCommand<
    TSource, TSourcePredicate, T, TPredicate,
    TOriginalCommand, TValueLifter, TPredicateLifter, TValueEmbeder, TPostConditionSemiGroup>
    : IHoareTripleCommand<T, TPredicate>
    where TOriginalCommand : IHoareTripleCommand<TSource, TSourcePredicate>
    where TValueLifter : IHoareTripleMorphism<TSource, TSourcePredicate, T, TPredicate>
    where TPredicateLifter : IHoareTripleMorphism<TSourcePredicate, TPredicate, TPredicate, TPredicate>
    where TValueEmbeder : IHoareTripleMorphism<T, TPredicate, TSource, TSourcePredicate>
    where TPostConditionSemiGroup : ISemiGroup<TPredicate>
{
    private readonly TPredicate _precondition;
    private readonly TValueEmbeder _valueEmbeder;
    private readonly TOriginalCommand _originalCommand;
    private readonly TValueLifter _valueLifter;
    private readonly TPredicateLifter _predicateLifter;
    private readonly TPostConditionSemiGroup _postConditionSemiGroup;
    
    public HoareTripleLiftedCommand
    (
        TPredicate precondition,
        TValueEmbeder valueEmbeder,
        TOriginalCommand originalCommand, 
        TValueLifter valueLifter, 
        TPredicateLifter predicateLifter,
        TPostConditionSemiGroup postConditionSemiGroup
    )
    {
        _originalCommand = originalCommand;
        _valueLifter = valueLifter;
        _predicateLifter = predicateLifter;
        _precondition = precondition;
        _valueEmbeder = valueEmbeder;
        _postConditionSemiGroup = postConditionSemiGroup;
    }

    public ref readonly TPredicate PreCondition => ref _precondition;

    public T Invoke
    (
        scoped ref readonly T value, 
        out TPredicate postCondition5
    )
    {
        // Assert: PreCondition 이 originalCommand 의 precondition 을 만족하지 않는 embedded 를 모두 걸러내야 한다!
        // TODO: 위 표명을 디버그 또는 테스트로 (즉, 경험적으로) 확인할 방법?
        TSource embedded = _valueEmbeder.Morph(in value, out _ /*TSourcePredicate sourcePostCondition1*/); 
        TSource result = _originalCommand.Invoke(in embedded, out TSourcePredicate sourcePostCondition2);
        T liftedResult = _valueLifter.Morph(in result, out TPredicate postCondition3);
        //TPredicate postCondition1 = _predicateLifter.Morph(in sourcePostCondition1, out TPredicate postCondition4);
        TPredicate postCondition2 = _predicateLifter.Morph(in sourcePostCondition2, out postCondition5);
        
        //_postConditionSemiGroup.Compose(in postCondition5, in postCondition4, ref postCondition5);
        _postConditionSemiGroup.Compose(in postCondition5, in postCondition3, ref postCondition5);
        _postConditionSemiGroup.Compose(in postCondition5, in postCondition2, ref postCondition5);
        //_postConditionSemiGroup.Compose(in postCondition5, in postCondition1, ref postCondition5);
        return liftedResult;
    }
}
