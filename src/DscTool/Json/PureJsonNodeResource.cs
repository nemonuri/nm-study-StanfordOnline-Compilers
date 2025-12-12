using System.Runtime.InteropServices;
using System.Text.Json.Nodes;

namespace DscTool.Json;

[StructLayout(LayoutKind.Sequential)]
public readonly record struct PureJsonNodeResource(JsonNode? Self) : IJsonNodeResource
{
    public JsonNode? ToJsonNode() => Self;
}
