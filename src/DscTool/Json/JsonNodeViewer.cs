using System.Text.Json;
using System.Text.Json.Nodes;

namespace DscTool.Json;

public struct JsonNodeViewer : IJsonNodeState<JsonNodeViewer>
{
    private readonly JsonNode? _jsonNode;
    private volatile JsonNodeViewer[]? _childrenLazy;
    private JsonNodeViewer[] ChildrenLazy
    {
        get
        {
            return _childrenLazy ??= Interlocked.CompareExchange(ref _childrenLazy, CreateCore(_jsonNode), null) ?? _childrenLazy;

            static JsonNodeViewer[] CreateCore(JsonNode? jn)
            {
                if (jn is JsonArray jsonArray)
                {
                    int count = 0;
                    foreach (var item in jsonArray) { if (item is not null) { count++; } }
                    var nodes = new JsonNodeViewer[count];
                    int index = 0;
                    foreach (var item in jsonArray)
                    {
                        if (item is not null)
                        {
                            nodes[index] = new(item);
                            index++;
                        }
                    }
                    return nodes;
                }
                else if (jn is JsonObject jsonObject)
                {
                    int count = 0;
                    foreach (var item in jsonObject) { if (item.Value is not null) { count++; } }
                    var nodes = new JsonNodeViewer[count];
                    int index = 0;
                    foreach (var item in jsonObject)
                    {
                        if (item.Value is { } ensuredValue)
                        {
                            nodes[index] = new(ensuredValue);
                            index++;
                        }
                    }
                    return nodes;
                }
                else
                {
                    return [];
                }
            }
        }
    }

    public readonly JsonNode? JsonNode => _jsonNode;

    public JsonNodeViewer(JsonNode? jsonNode)
    {
        _jsonNode = jsonNode;
        _childrenLazy = null;
    }

    public ReadOnlySpan<JsonNodeViewer> GetChildrenAsReadOnlySpan() => new (ChildrenLazy);

    [MemberNotNullWhen(true, nameof(JsonNode))]
    public readonly bool Exist => _jsonNode is not null;

    [MemberNotNullWhen(true, nameof(JsonNode))]
    public readonly bool IsValidJson => !(_jsonNode?.GetValueKind() is null or JsonValueKind.Undefined);
}
