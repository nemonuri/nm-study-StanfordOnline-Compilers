using DscTool.Infrastructure;

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
    OptionalGraphed<JsonNodeId, TValue> OptionalGraphed,
    JsonNodeId RootId
)
{
    public bool TryGetNodeId(out JsonNodeId jsonNodeId) => OptionalGraphed.TryGetNodeKey(out jsonNodeId);

    public TValue Value => OptionalGraphed.OptionalKeyedEntry.Value;
}


public static class JsonGraphed
{
    public static JsonGraphed<TValue> CreateFromPureValue<TValue>(TValue pureValue)
    {
        return new JsonGraphed<TValue>
        (
            OptionalGraphed: new (OptionalGraph: null, new (key: OptionalKeyTagger.None, value: pureValue)),
            RootId: default
        );
    }
}
