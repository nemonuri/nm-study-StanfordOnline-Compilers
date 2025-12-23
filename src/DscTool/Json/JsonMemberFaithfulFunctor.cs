using System.Text.Json.Nodes;
using DscTool.Infrastructure;
using DscTool.Scoped;
using DscTool.Scoped.Hashtables;
using Json.Pointer;
using Json.Schema;

namespace DscTool.Json;

public readonly struct JsonMemberConditionLifter : IScopedPackedMapConditionLifter<JsonGraphed<JsonAtomicSchema>, JsonMemberSegment>
{
    private static readonly JsonGraphed<JsonAtomicSchema> s_maxJoin = JsonGraphed.CreateFromPureValue(JsonAtomicSchema.MaxJoin);

    public ref readonly JsonGraphed<JsonAtomicSchema> PreCondition => ref s_maxJoin;

    public bool TryMorph
    (
        scoped ref readonly PackedMap<JsonMemberSegment, JsonGraphed<JsonAtomicSchema>> source, 
        [NotNullWhen(true)] scoped ref JsonGraphed<JsonAtomicSchema> result, 
        [NotNullWhen(true)] scoped ref JsonGraphed<JsonAtomicSchema> postCondition)
    {

    }
}

public readonly struct JsonMemberLifter : IScopedPackedMapLifter<JsonGraphed<JsonAtomicValue>, JsonGraphed<JsonAtomicSchema>, JsonMemberSegment>
{
    public ref readonly PackedMap<JsonMemberSegment, JsonGraphed<JsonAtomicSchema>> PreCondition => throw new NotImplementedException();

    public bool TryMorph
    (
        scoped ref readonly PackedMap<JsonMemberSegment, JsonGraphed<JsonAtomicValue>> source, 
        [NotNullWhen(true)] scoped ref JsonGraphed<JsonAtomicValue> result, 
        [NotNullWhen(true)] scoped ref JsonGraphed<JsonAtomicSchema> postCondition
    )
    {
        throw new NotImplementedException();
    }
}

public readonly struct JsonMemberEmbedder : IScopedPackedMapEmbedder<JsonGraphed<JsonAtomicValue>, JsonGraphed<JsonAtomicSchema>, JsonMemberSegment>
{
    public ref readonly JsonGraphed<JsonAtomicSchema> PreCondition => throw new NotImplementedException();

    public bool TryMorph
    (
        scoped ref readonly JsonGraphed<JsonAtomicValue> source, 
        [NotNullWhen(true)] scoped ref PackedMap<JsonMemberSegment, JsonGraphed<JsonAtomicValue>> result, 
        [NotNullWhen(true)] scoped ref PackedMap<JsonMemberSegment, JsonGraphed<JsonAtomicSchema>> postCondition
    )
    {
        throw new NotImplementedException();
    }

    
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
