namespace Nemonuri.Json.LowLevel;

public readonly struct JsonEdgeLabel : IEquatable<JsonEdgeLabel>
{
    private readonly LowLevelChoice<int, string> _choice;

    public bool Equals(JsonEdgeLabel other) => _choice.Equals(other._choice);

    public override bool Equals(object? obj) => obj is JsonEdgeLabel v && Equals(v);
    
    public override int GetHashCode() => _choice.GetHashCode();
}
