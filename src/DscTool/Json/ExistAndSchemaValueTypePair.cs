using Json.Schema;

namespace DscTool.Json;

public readonly record struct ExistAndSchemaValueTypePair(bool IsExist, SchemaValueType SchemaValueType);
