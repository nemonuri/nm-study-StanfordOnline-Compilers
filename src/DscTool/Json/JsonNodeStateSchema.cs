using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using Json.Schema;

namespace DscTool.Json;

public readonly struct JsonNodeSchemaViewer : IJsonNodeStateSchema<JsonNodeViewer, JsonNodeSchemaViewer>
{
    private readonly JsonSchema? _jsonSchema;
    public readonly JsonSchema? JsonSchema => _jsonSchema;

    public JsonNodeSchemaViewer(JsonSchema? jsonSchema)
    {
        _jsonSchema = jsonSchema;
    }

    [MemberNotNullWhen(true, nameof(JsonSchema))]
    public readonly bool Exist => _jsonSchema is not null;

    [MemberNotNullWhen(true, nameof(JsonSchema))]
    public readonly bool IsValidJson => Exist;

    public readonly bool TryMorph
    (
        scoped ref readonly JsonNodeViewer sourceRoot, 
        int sourceChildIndex, 
        [NotNullWhen(true)] out JsonNodeViewer sourceResult, 
        [NotNullWhen(true)] out JsonNodeSchemaViewer targetResult, 
        out TreeMorphErrorKind errorKind
    )
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool ReturnError
        (
            TreeMorphErrorKind inputErrorKind,
            out JsonNodeViewer sourceResult, out JsonNodeSchemaViewer targetResult, out TreeMorphErrorKind errorKind
        )
        {
            (sourceResult, targetResult, errorKind) = (default, default, inputErrorKind);
            return false;
        }

        if (!this.IsValidJson)
        {
            return ReturnError(new(TreeWalkErrorKind.InvalidRoot, MorphErrorSide.Target), out sourceResult, out targetResult, out errorKind);
        }
        if (!sourceRoot.IsValidJson) 
        { 
            return ReturnError(new(TreeWalkErrorKind.InvalidRoot, MorphErrorSide.Source), out sourceResult, out targetResult, out errorKind);
        }

        ReadOnlySpan<JsonNodeViewer> sourceChildren = sourceRoot.GetChildrenAsReadOnlySpan();
        if (sourceChildren.IsEmpty)
        {
            return ReturnError(new(TreeWalkErrorKind.OutOfDepth, MorphErrorSide.Source), out sourceResult, out targetResult, out errorKind);
        }

        if (!(sourceChildren.Length > sourceChildIndex))
        {
            return ReturnError(new(TreeWalkErrorKind.OutOfWidth, MorphErrorSide.Source), out sourceResult, out targetResult, out errorKind);
        }

        sourceResult = sourceChildren[sourceChildIndex];

        if (sourceResult.JsonNode is { } jn)
        {
            if (jn.Parent is JsonObject)
            {
                if(!(JsonSchema.GetProperties()?.TryGetValue(jn.GetPropertyName(), out JsonSchema? childJsonSchema) ?? false))
                {
                    return ReturnError(new(TreeWalkErrorKind.OutOfDepthOrWidth, MorphErrorSide.Target), out sourceResult, out targetResult, out errorKind);
                }

                (targetResult, errorKind) = (new(childJsonSchema), default);
                return true;
            }
            else if (jn.Parent is JsonArray)
            {
                if (!(JsonSchema.GetItems() is { } childJsonSchema))
                {
                    return ReturnError(new(TreeWalkErrorKind.InvalidRoot, MorphErrorSide.Target), out sourceResult, out targetResult, out errorKind);
                }

                (targetResult, errorKind) = (new(childJsonSchema), default);
                return true;
            }   
        }

        // Assert: child of 'sourceRoot' is invalid.
        return ReturnError(new(TreeWalkErrorKind.InvalidChild, MorphErrorSide.Source), out sourceResult, out targetResult, out errorKind);
    }
}

