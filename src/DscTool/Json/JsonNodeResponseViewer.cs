
using Json.Schema;

namespace DscTool.Json;

public struct JsonNodeResponseViewer : IJsonNodeResponse<JsonNodeViewer, JsonNodeSchemaViewer, JsonNodeResponseViewer>
{
    private volatile EvaluationResults? _evaluationResults;
    private readonly JsonNodeViewer _equivalentState;
    private readonly JsonNodeSchemaViewer _equivalentStateSchema;

    public JsonNodeResponseViewer(JsonNodeViewer equivalentState, JsonNodeSchemaViewer equivalentStateSchema)
    {
        _evaluationResults = default;
        _equivalentState = equivalentState;
        _equivalentStateSchema = equivalentStateSchema;
    }

    public EvaluationResults EvaluationResults
    {
        get
        {
            return _evaluationResults ??= Interlocked.CompareExchange(ref _evaluationResults, CreateCore(in this), null) ?? _evaluationResults;

            static EvaluationResults CreateCore(scoped ref readonly JsonNodeResponseViewer self)
            {
                EvaluationOptions eo = new() { OutputFormat = OutputFormat.Hierarchical };
                if (!self._equivalentStateSchema.IsValidJson) { return JsonSchema.False.Evaluate(null, eo); }

                return self._equivalentStateSchema.JsonSchema.Evaluate(self._equivalentState.JsonNode, eo);
            }
        }
    }

    [UnscopedRef] public readonly ref readonly JsonNodeViewer EquivalentState => ref _equivalentState;

    [UnscopedRef] public readonly ref readonly JsonNodeSchemaViewer EquivalentStateSchema => ref _equivalentStateSchema;

    private volatile JsonNodeResponseViewer[]? _childrenLazy = default;
    private JsonNodeResponseViewer[] ChildrenLazy
    {
        get
        {
            return _childrenLazy ??= Interlocked.CompareExchange(ref _childrenLazy, CreateCore(in this), null) ?? _childrenLazy;

            static JsonNodeResponseViewer[] CreateCore(scoped ref readonly JsonNodeResponseViewer self)
            {
                ReadOnlySpan<JsonNodeViewer> stateChildren = self._equivalentState.GetChildrenAsReadOnlySpan();
                var chilren = new JsonNodeResponseViewer[stateChildren.Length];

                for (int i = 0; i < stateChildren.Length; i++)
                {
                    self._equivalentStateSchema.TryMorph(in self._equivalentState, i, out var state, out var schema, out _);
                    chilren[i] = new(state, schema);
                }

                return chilren;
            }
        }
    }


    public ReadOnlySpan<JsonNodeResponseViewer> GetChildrenAsReadOnlySpan() => new(ChildrenLazy);

    public bool InDesiredState => EvaluationResults.IsValid;
}

