using System.Text.Json.Nodes;

namespace DscTool.Json;

public readonly record struct JsonNodeAndPathSegmentPair(JsonNode? JsonNode, string PathSegment)
{
    public JsonNode? GetMemberOrNull()
    {
        return JsonNode switch
        {
            JsonObject jo => jo[PathSegment],
            _ => null
        };
    }
};
