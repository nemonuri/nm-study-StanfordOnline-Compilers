
using System.Text.Json;
using System.Text.Json.Nodes;
using DscTool.Scoped;
using Json.Schema;

namespace DscTool.Json;

public readonly struct JsonGraphedAtomicCategory : 
    IScopedCategory<JsonGraphed<JsonAtomicValue>, JsonGraphed<JsonAtomicSchema>>
{
    private readonly JsonAtomicCategory _atomic;

    public bool Equals(JsonGraphed<JsonAtomicValue> x, JsonGraphed<JsonAtomicValue> y)
    {
        return x.GraphedValue.Entry.Key.Equals(y.GraphedValue.Entry.Key);
    }

    public int GetHashCode(JsonGraphed<JsonAtomicValue> obj)
    {
        return obj.GraphedValue.Entry.Key.GetHashCode();
    }

    public bool Satisfies(scoped ref readonly JsonGraphed<JsonAtomicValue> value, scoped ref readonly JsonGraphed<JsonAtomicSchema> condition)
    {
        var v1 = value.GraphedValue.Entry.Value;
        var v2 = condition.GraphedValue.Entry.Value;
        return _atomic.Satisfies(in v1, in v2);
    }

    public bool IsSufficient(scoped ref readonly JsonGraphed<JsonAtomicSchema> sufficient, scoped ref readonly JsonGraphed<JsonAtomicSchema> necessary)
    {
        var v1 = sufficient.GraphedValue.Entry.Value;
        var v2 = necessary.GraphedValue.Entry.Value;
        return _atomic.IsSufficient(in v1, in v2);
    }
}

