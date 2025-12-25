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
    public static readonly JsonAtomicValue Empty = new(JsonContainerKind.Unknown, default);

    public bool IsExist => JsonContainerKind switch
    {
        JsonContainerKind.Leaf or JsonContainerKind.Object or JsonContainerKind.Array => true,
        _ => false
    };
};

public readonly record struct JsonAtomicSchema(bool IsExist, SchemaValueType SchemaValueType)
{
    public static readonly JsonAtomicSchema MaxJoin = new(true, SchemaValueTypeTheory.ValidateMask);
};

public static class SchemaValueTypeTheory
{
    public const SchemaValueType ValidateMask = SchemaValueType.Object | SchemaValueType.Array | SchemaValueType.Boolean | SchemaValueType.String |
                                             SchemaValueType.Number | SchemaValueType.Integer | SchemaValueType.Null;
    
    extension(SchemaValueType)
    {
        public static SchemaValueType ValidateMask => SchemaValueTypeTheory.ValidateMask;
    }
}