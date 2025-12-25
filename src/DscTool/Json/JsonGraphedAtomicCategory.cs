
using System.Text.Json;
using System.Text.Json.Nodes;
using DscTool.Infrastructure;
using DscTool.Scoped;
using DscTool.Scoped.Hashtables;
using Json.Schema;

namespace DscTool.Json;

public readonly struct JsonGraphedAtomicCategory : 
    IScopedCategory<JsonGraphed<JsonAtomicValue>, JsonGraphed<JsonAtomicSchema>>
{
    private readonly JsonAtomicCategory _atomic;

    public bool Equals(JsonGraphed<JsonAtomicValue> x, JsonGraphed<JsonAtomicValue> y)
    {
        if (x.TryGetNodeId(out var nodeIdX) && y.TryGetNodeId(out var nodeIdY))
        {
            return nodeIdX.Equals(nodeIdY);
        }
        
        return _atomic.Equals(x.Value, y.Value);
    }

    public int GetHashCode(JsonGraphed<JsonAtomicValue> x)
    {
        if (x.TryGetNodeId(out var nodeId))
        {
            return nodeId.GetHashCode();
        }

        return _atomic.GetHashCode(x.Value);
    }

    public bool Satisfies(scoped ref readonly JsonGraphed<JsonAtomicValue> value, scoped ref readonly JsonGraphed<JsonAtomicSchema> condition)
    {
        var v1 = value.Value;
        var v2 = condition.Value;
        return _atomic.Satisfies(in v1, in v2);
    }

    public bool IsSufficient(scoped ref readonly JsonGraphed<JsonAtomicSchema> sufficient, scoped ref readonly JsonGraphed<JsonAtomicSchema> necessary)
    {
        var v1 = sufficient.Value;
        var v2 = necessary.Value;
        return _atomic.IsSufficient(in v1, in v2);
    }
}
