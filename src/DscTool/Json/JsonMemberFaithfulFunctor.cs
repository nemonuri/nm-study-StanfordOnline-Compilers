using System.Text.Json.Nodes;
using DscTool.Infrastructure;
using DscTool.Scoped;
using DscTool.Scoped.Hashtables;
using Json.Pointer;
using Json.Schema;

namespace DscTool.Json;

public readonly record struct JsonMemberSegment(ValueChoice<string, int> ValueChoice)
{
    public bool IsNone => ValueChoice.IsNone;

    public bool IsPropertyName => ValueChoice.IsChoice1;
    public string GetPropertyName() => ValueChoice.GetChoice1();

    public bool IsElementIndex => ValueChoice.IsChoice2;
    public int GetElementIndex() => ValueChoice.GetChoice2();
};

public readonly record struct JsonNodeId(int Id);

public readonly record struct JsonGraphed<TValue>
(
    Graphed<JsonNodeId, JsonMemberSegment, TValue> GraphedValue,
    JsonNodeId RootId
);

public readonly struct JsonMemberConditionLifter : IScopedPackedMapConditionLifter<JsonGraphed<JsonAtomicSchema>, JsonMemberSegment>
{
    private static readonly 

    public ref readonly JsonGraphed<JsonAtomicSchema> PreCondition => throw new NotImplementedException();

    public bool TryMorph
    (
        scoped ref readonly PackedMap<JsonMemberSegment, JsonGraphed<JsonAtomicSchema>> source, 
        [NotNullWhen(true)] scoped ref JsonGraphed<JsonAtomicSchema> result, 
        [NotNullWhen(true)] scoped ref JsonGraphed<JsonAtomicSchema> postCondition)
    {
        throw new NotImplementedException();
    }
}

public readonly struct JsonMemberLifter : IScopedPackedMapLifter<JsonNodeAndPathSegmentPair, JsonAtomicSchema>
{
    public bool TryMorph
    (
        scoped ref readonly Memory<JsonNodeAndPathSegmentPair> source, 
        [NotNullWhen(true)] scoped ref JsonNodeAndPathSegmentPair result, 
        [NotNullWhen(true)] scoped ref JsonAtomicSchema postCondition
    )
    {
        throw new NotImplementedException();
    }

    public ref readonly Memory<JsonAtomicSchema> PreCondition => throw new NotImplementedException();
}

public readonly struct JsonMemberEmbedder : IScopedPackedMapEmbedder<JsonNodeAndPathSegmentPair, JsonAtomicSchema>
{
    public bool TryMorph
    (
        scoped ref readonly JsonNodeAndPathSegmentPair source, 
        [NotNullWhen(true)] scoped ref Memory<JsonNodeAndPathSegmentPair> result, 
        [NotNullWhen(true)] scoped ref Memory<JsonAtomicSchema> postCondition
    )
    {
        throw new NotImplementedException();
    }

    public ref readonly JsonAtomicSchema PreCondition => throw new NotImplementedException();
}

public readonly struct JsonMemberFaithfulFunctor : 
    IScopedPackedMapFaithfulFunctor<
        JsonNode, JsonAtomicSchema,
        JsonMemberCategory_, 
        JsonMemberConditionLifter, 
        JsonMemberLifter, 
        JsonMemberEmbedder>
{
    private readonly
    ScopedPackedMapFaithfulFunctor<
    JsonNodeAndPathSegmentPair, JsonAtomicSchema,
    JsonMemberCategory_, 
    JsonMemberConditionLifter, 
    JsonMemberLifter, 
    JsonMemberEmbedder> 
    _functor;

    public JsonMemberFaithfulFunctor()
    {
        _functor = new(default, default, default, default);
    }

    [UnscopedRef] public ref readonly JsonMemberCategory_ TargetCategory => ref _functor.TargetCategory;

    [UnscopedRef] public ref readonly JsonMemberConditionLifter ConditionLifter => ref _functor.ConditionLifter;

    [UnscopedRef] public ref readonly JsonMemberLifter Lifter => ref _functor.Lifter;

    [UnscopedRef] public ref readonly JsonMemberEmbedder Embedder => ref _functor.Embedder;


    public bool Satisfies(scoped ref readonly Memory<JsonAtomicSchema> value, scoped ref readonly JsonAtomicSchema condition) =>
        _functor.Satisfies(in value, in condition);

    public bool IsSufficient(scoped ref readonly JsonAtomicSchema sufficient, scoped ref readonly JsonAtomicSchema necessary) =>
        _functor.IsSufficient(in sufficient, in necessary);
}
