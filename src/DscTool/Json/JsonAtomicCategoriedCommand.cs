using System.Text.Json.Nodes;
using DscTool.Scoped;
using Json.Schema;

namespace DscTool.Json;

public class JsonAtomicCategoriedCommand : IScopedCategoriedCommand<JsonAtomicValue, JsonAtomicSchema, JsonAtomicCategory>
{
    private readonly JsonAtomicSchema _preCondition;
    private readonly JsonAtomicSchema _desiredCondition;

    public JsonAtomicCategoriedCommand(JsonAtomicSchema preCondition, JsonAtomicSchema desiredCondition)
    {
        _preCondition = preCondition;
        _desiredCondition = desiredCondition;
    }

    public ref readonly JsonAtomicCategory Category => ref JsonAtomicCategory.Instance;

    public ref readonly JsonAtomicSchema PreCondition => ref _preCondition;

    public bool TryInvoke
    (
        scoped ref readonly JsonAtomicValue source, 
        [NotNullWhen(true)] scoped ref JsonAtomicValue result, 
        [NotNullWhen(true)] scoped ref JsonAtomicSchema postCondition
    )
    {
        if (Category.Satisfies(in source, in _desiredCondition))
        {
            result = source;
            postCondition = _desiredCondition;
            return true;
        }
        if (!_desiredCondition.IsExist)
        {
            result = JsonAtomicValue.Empty;
            postCondition = _desiredCondition;
            return true;
        }

        JsonAtomicValue? resultCandidate = _desiredCondition.SchemaValueType switch
        {
            var v when (v & SchemaValueType.Object) is not 0 => new(JsonContainerKind.Object, default),
            var v when (v & SchemaValueType.Array) is not 0 => new(JsonContainerKind.Array, default),
            var v when (v & SchemaValueType.Boolean) is not 0 => new(JsonContainerKind.Leaf, JsonValue.Create(false)),
            var v when (v & SchemaValueType.String) is not 0 => new(JsonContainerKind.Leaf, JsonValue.Create("")),
            var v when (v & (SchemaValueType.Number | SchemaValueType.Integer)) is not 0 => new(JsonContainerKind.Leaf, JsonValue.Create(0)),
            var v when (v & SchemaValueType.Null) is not 0 => new(JsonContainerKind.Leaf, default),
            _ => null
        };

        if (!resultCandidate.HasValue) {return false;}

        result = resultCandidate.Value;
        postCondition = _desiredCondition;
        return true;
    }
}
