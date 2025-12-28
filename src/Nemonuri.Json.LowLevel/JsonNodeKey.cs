
namespace Nemonuri.Json.LowLevel;

public readonly partial struct JsonNodeKey : IReadOnlyMemoryView<JsonEdgeLabel>
{
    private readonly ImmutableArrayView<JsonEdgeLabel> _arrayView;

    internal JsonNodeKey(ImmutableArray<JsonEdgeLabel> array)
    {
        _arrayView = new(array);
    }

    public int Length => _arrayView.Length;

    public ref readonly JsonEdgeLabel this[int index] => ref _arrayView[index];
}