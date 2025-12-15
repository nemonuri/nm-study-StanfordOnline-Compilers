using System.Diagnostics;
using System.Text.Json.Nodes;
using DscTool.Scoped;
using Json.Schema;

namespace DscTool.Json;

public class JsonMemberSimpleEditor : IScopedCategoriedCommand<JsonNodeAndPathSegmentPair, ExistAndSchemaValueTypePair, JsonMemberCategory>
{
    private readonly ExistAndSchemaValueTypePair _preCondition;
    private readonly ExistAndSchemaValueTypePair _desiredCondition;

    public JsonMemberSimpleEditor(ExistAndSchemaValueTypePair preCondition, ExistAndSchemaValueTypePair desiredCondition)
    {
        _preCondition = preCondition;
        _desiredCondition = desiredCondition;
    }

    public ref readonly JsonMemberCategory Category => ref JsonMemberCategory.Instance;

    public ref readonly ExistAndSchemaValueTypePair PreCondition => ref _preCondition;

    public bool TryInvoke
    (
        scoped ref readonly JsonNodeAndPathSegmentPair source, 
        [NotNullWhen(true)] scoped ref JsonNodeAndPathSegmentPair result, 
        [NotNullWhen(true)] scoped ref ExistAndSchemaValueTypePair postCondition
    )
    {
        if (Category.Satisfies(in source, in _desiredCondition))
        {
            result = source;
            postCondition = _desiredCondition;
            return true;
        }
        var foundMemberNode = source.GetMemberOrNull();
        if (foundMemberNode is null && source.JsonNode is JsonObject jo)
        {
            Debug.Assert(_desiredCondition.IsExist);
            jo.Add(source.PathSegment, CreateFromSchemaValueType(_desiredCondition.SchemaValueType));

            result = source;
            postCondition = _desiredCondition;
            return true;
        }
        
        return false;
    }

    private static JsonNode? CreateFromSchemaValueType(SchemaValueType schemaValueType)
    {
        return schemaValueType switch
        {
            SchemaValueType.Array => new JsonArray(),
            SchemaValueType.Boolean => JsonValue.Create(false),
            SchemaValueType.String => JsonValue.Create(""),
            SchemaValueType.Number => JsonValue.Create(0.0),
            SchemaValueType.Integer => JsonValue.Create(0),
            SchemaValueType.Object => new JsonObject(),
            _ => null
        };
    }    
}
