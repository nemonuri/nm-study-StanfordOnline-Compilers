using System.Text.Json.Nodes;
using Json.Schema;

namespace DscTool.Json;

public class PureJsonNodeDscComponent :
    IDscComponent<
        int, ImmutableArray<int>, 
        JsonNodeViewer, Nil,
        JsonNodeSchemaViewer, Nil,
        JsonNodeResponseViewer, Nil>
{
    private readonly static StateSchemaPair<Nil, Nil> s_NullConditionPair = default;

    private readonly ImmutableArray<int> _precondition;
    public ref readonly ImmutableArray<int> PreCondition => ref _precondition;

    private readonly Dictionary<int, StateSchemaPair<JsonNode, JsonSchema>> _store;
    
    public PureJsonNodeDscComponent(Dictionary<int, StateSchemaPair<JsonNode, JsonSchema>> store)
    {
        Guard.IsNotNull(store);
        _store = store;
        _precondition = [.._store.Keys];
    }

    public JsonNodeViewer Morph(scoped ref readonly TagState<int> source, out Nil postCondition)
    {
        postCondition = default;
        return new (_store[source.Self].State); 
    }

    public JsonNodeSchemaViewer Morph(scoped ref readonly TagStateSchema<int> source, out Nil postCondition)
    {
        postCondition = default;
        return new (_store[source.Self].StateSchema); 
    }

    public JsonNodeResponseViewer Morph(scoped ref readonly TagTest<StateSchemaPair<JsonNodeViewer, JsonNodeSchemaViewer>> source, out Nil postCondition)
    {
        postCondition = default;
        return new JsonNodeResponseViewer(source.Self.State, source.Self.StateSchema);
    }

    public JsonNodeResponseViewer Morph(scoped ref readonly TagEdit<StateSchemaPair<JsonNodeViewer, JsonNodeSchemaViewer>> source, out Nil postCondition)
    {
        throw new NotImplementedException();
    }


    ref readonly StateSchemaPair<Nil, Nil> IHoareTripleMorphism<TagTest<StateSchemaPair<JsonNodeViewer, JsonNodeSchemaViewer>>, StateSchemaPair<Nil, Nil>, JsonNodeResponseViewer, Nil>.PreCondition => ref s_NullConditionPair;
    ref readonly StateSchemaPair<Nil, Nil> IHoareTripleMorphism<TagEdit<StateSchemaPair<JsonNodeViewer, JsonNodeSchemaViewer>>, StateSchemaPair<Nil, Nil>, JsonNodeResponseViewer, Nil>.PreCondition => ref s_NullConditionPair;

}
