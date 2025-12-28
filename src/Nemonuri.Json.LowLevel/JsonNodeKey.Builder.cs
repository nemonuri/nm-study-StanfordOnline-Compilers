
namespace Nemonuri.Json.LowLevel;

public readonly partial struct JsonNodeKey
{
    public static Builder CreateBuilder() => new();

    public class Builder : IMemoryView<JsonEdgeLabel>
    {
        private readonly ImmutableArray<JsonEdgeLabel>.Builder _builder;

        internal Builder()
        {
            _builder = ImmutableArray.CreateBuilder<JsonEdgeLabel>();
        }

        public int Length => _builder.Count;

        public ref JsonEdgeLabel this[int index] => ref Unsafe.AsRef(in _builder.ItemRef(index));

        public JsonNodeKey ToJsonNodeKey()
        {
            return new(_builder.ToImmutable());
        }
    }
}