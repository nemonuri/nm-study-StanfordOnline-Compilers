using DscTool.Scoped;
using DscTool.Scoped.Sequences;

namespace DscTool.Json;

public readonly struct JsonMemberConditionLifter : IScopedMemoryConditionLifter<ExistAndSchemaValueTypePair>
{
    public bool TryMorph
    (
        scoped ref readonly Memory<ExistAndSchemaValueTypePair> source, 
        [NotNullWhen(true)] scoped ref ExistAndSchemaValueTypePair result, 
        [NotNullWhen(true)] scoped ref ExistAndSchemaValueTypePair postCondition
    )
    {
        throw new NotImplementedException();
    }

    public ref readonly ExistAndSchemaValueTypePair PreCondition => throw new NotImplementedException();
}

public readonly struct JsonMemberLifter : IScopedMemoryLifter<JsonNodeAndPathSegmentPair, ExistAndSchemaValueTypePair>
{
    public bool TryMorph
    (
        scoped ref readonly Memory<JsonNodeAndPathSegmentPair> source, 
        [NotNullWhen(true)] scoped ref JsonNodeAndPathSegmentPair result, 
        [NotNullWhen(true)] scoped ref ExistAndSchemaValueTypePair postCondition
    )
    {
        throw new NotImplementedException();
    }

    public ref readonly Memory<ExistAndSchemaValueTypePair> PreCondition => throw new NotImplementedException();
}

public readonly struct JsonMemberEmbedder : IScopedMemoryEmbedder<JsonNodeAndPathSegmentPair, ExistAndSchemaValueTypePair>
{
    public bool TryMorph
    (
        scoped ref readonly JsonNodeAndPathSegmentPair source, 
        [NotNullWhen(true)] scoped ref Memory<JsonNodeAndPathSegmentPair> result, 
        [NotNullWhen(true)] scoped ref Memory<ExistAndSchemaValueTypePair> postCondition
    )
    {
        throw new NotImplementedException();
    }

    public ref readonly ExistAndSchemaValueTypePair PreCondition => throw new NotImplementedException();
}

public readonly struct JsonMemberFaithfulFunctor : 
    IScopedMemoryFaithfulFunctor<
        JsonNodeAndPathSegmentPair, ExistAndSchemaValueTypePair,
        JsonMemberCategory, 
        JsonMemberConditionLifter, 
        JsonMemberLifter, 
        JsonMemberEmbedder>
{
    private readonly
    ScopedMemoryFaithfulFunctor<
    JsonNodeAndPathSegmentPair, ExistAndSchemaValueTypePair,
    JsonMemberCategory, 
    JsonMemberConditionLifter, 
    JsonMemberLifter, 
    JsonMemberEmbedder> 
    _functor;

    public JsonMemberFaithfulFunctor()
    {
        _functor = new(default, default, default, default);
    }

    [UnscopedRef] public ref readonly JsonMemberCategory TargetCategory => ref _functor.TargetCategory;

    [UnscopedRef] public ref readonly JsonMemberConditionLifter ConditionLifter => ref _functor.ConditionLifter;

    [UnscopedRef] public ref readonly JsonMemberLifter Lifter => ref _functor.Lifter;

    [UnscopedRef] public ref readonly JsonMemberEmbedder Embedder => ref _functor.Embedder;


    public bool Satisfies(scoped ref readonly Memory<ExistAndSchemaValueTypePair> value, scoped ref readonly ExistAndSchemaValueTypePair condition) =>
        _functor.Satisfies(in value, in condition);

    public bool IsSufficient(scoped ref readonly ExistAndSchemaValueTypePair sufficient, scoped ref readonly ExistAndSchemaValueTypePair necessary) =>
        _functor.IsSufficient(in sufficient, in necessary);
}
