using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Schema;

namespace DscTool.Json;

public enum JsonContainerKind
{
    Unknown = 0,
    Leaf = 1,
    Object = 2,
    Array = 3
}

public readonly record struct JsonAtomicValue(JsonContainerKind JsonContainerKind, JsonValue? JsonValue)
{
    public bool IsExist => JsonContainerKind switch
    {
        JsonContainerKind.Leaf => JsonValue is not null,
        JsonContainerKind.Object or JsonContainerKind.Array => true,
        _ => false
    };
};

public readonly record struct JsonAtomicSchema(bool IsExist, SchemaValueType SchemaValueType);

