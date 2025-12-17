
using System.Text.Json;
using System.Text.Json.Nodes;
using DscTool.Scoped;
using Json.Schema;

namespace DscTool.Json;

public readonly struct JsonMemberCategory : IScopedCategory<JsonNodeAndPathSegmentPair, ExistAndSchemaValueTypePair>
{
    public readonly static JsonMemberCategory Instance = default;

    public bool Equals(JsonNodeAndPathSegmentPair x, JsonNodeAndPathSegmentPair y)
    {
        return x == y;
    }

    public int GetHashCode(JsonNodeAndPathSegmentPair obj)
    {
        return obj.GetHashCode();
    }

    public bool Satisfies(scoped ref readonly JsonNodeAndPathSegmentPair value, scoped ref readonly ExistAndSchemaValueTypePair condition)
    {
        JsonNode? foundNode = value.GetMemberOrNull();
        if (foundNode is null) 
        { 
            return condition.IsExist switch
            {
                true => false,
                false => true
            };
        }
        var schemaValueKind = GetSchemaValueType(foundNode);
        return IsSchemaValueTypeSufficient(schemaValueKind, condition.SchemaValueType);
    }

    public bool IsSufficient(scoped ref readonly ExistAndSchemaValueTypePair sufficient, scoped ref readonly ExistAndSchemaValueTypePair necessary)
    {
        return (sufficient.IsExist == necessary.IsExist) && IsSchemaValueTypeSufficient(sufficient.SchemaValueType, necessary.SchemaValueType);
    }

    private static SchemaValueType GetSchemaValueType(JsonNode node)
    {
        return node.GetValueKind() switch
        {
            JsonValueKind.Object => SchemaValueType.Object,
            JsonValueKind.Array => SchemaValueType.Array,
            JsonValueKind.String => SchemaValueType.String,
            JsonValueKind.Number => node.AsValue().TryGetValue<Int64>(out _) ? SchemaValueType.Integer : SchemaValueType.Number,
            JsonValueKind.True or JsonValueKind.False => SchemaValueType.Boolean,
            JsonValueKind.Null => SchemaValueType.Null,
            _ => default
        };
    }

    private static bool IsSchemaValueTypeSufficient(SchemaValueType sufficient, SchemaValueType necessary)
    {
        const SchemaValueType validateMask = SchemaValueType.Object | SchemaValueType.Array | SchemaValueType.Boolean | SchemaValueType.String |
                                             SchemaValueType.Number | SchemaValueType.Integer | SchemaValueType.Null;
        if (!((sufficient & validateMask) !=0 && (necessary & validateMask) !=0))
        {
            return false;
        }

        return (sufficient & necessary) != 0;
    }
}
