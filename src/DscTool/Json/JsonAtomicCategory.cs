
using System.Text.Json;
using System.Text.Json.Nodes;
using DscTool.Scoped;
using Json.Schema;

namespace DscTool.Json;

public readonly struct JsonAtomicCategory() : IScopedCategory<JsonAtomicValue, JsonAtomicSchema>
{
    public static readonly JsonAtomicCategory Instance = new();

    public bool Equals(JsonAtomicValue x, JsonAtomicValue y)
    {
        return (x.JsonContainerKind == y.JsonContainerKind) && 
               (ConvertJsonValueToString(x.JsonValue) == ConvertJsonValueToString(y.JsonValue));
    }

    public int GetHashCode(JsonAtomicValue obj)
    {
        HashCode hc = default;
        hc.Add(obj.JsonContainerKind);
        hc.Add(obj.JsonValue);
        return hc.ToHashCode();
    }

    public bool Satisfies(scoped ref readonly JsonAtomicValue value, scoped ref readonly JsonAtomicSchema condition)
    {
        if (value.IsExist != condition.IsExist) {return false;}

        if (value.JsonContainerKind == JsonContainerKind.Leaf)
        {
            if (value.JsonValue is not { } jsonValue) {return false;}

            return IsSchemaValueTypeSufficient(GetSchemaValueType(jsonValue), condition.SchemaValueType);
        }
        else
        {
            return IsSchemaValueTypeSufficient(GetSchemaValueTypeExceptLeaf(value.JsonContainerKind), condition.SchemaValueType);
        }
    }

    public bool IsSufficient(scoped ref readonly JsonAtomicSchema sufficient, scoped ref readonly JsonAtomicSchema necessary)
    {
        return (sufficient.IsExist == necessary.IsExist) && IsSchemaValueTypeSufficient(sufficient.SchemaValueType, necessary.SchemaValueType);
    }

    private static string ConvertJsonValueToString(JsonValue? jsonValue)
    {
        return jsonValue?.ToString() ?? "";
    }

    private static SchemaValueType GetSchemaValueTypeExceptLeaf(JsonContainerKind node)
    {
        return node switch
        {
            JsonContainerKind.Object => SchemaValueType.Object,
            JsonContainerKind.Array => SchemaValueType.Array,
            _ => default
        };
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
        const SchemaValueType validateMask = SchemaValueTypeTheory.ValidateMask;
        if (!((sufficient & validateMask) !=0 && (necessary & validateMask) !=0))
        {
            return false;
        }

        return (sufficient & necessary) != 0;
    }
}
