using System.Text.Json.Nodes;

namespace DscTool.Json;

public interface ISupportIsValidJson
{
    bool IsValidJson {get;}
}

public interface IJsonNodeResource
{
    JsonNode? ToJsonNode();
}

public interface IJsonNodeState<TState> : IState<TState>, ISupportIsValidJson
    where TState : IJsonNodeState<TState>
{
}

public interface IJsonNodeStateSchema<TState, TStateSchema> : 
    ISupportIsValidJson, IStateSchema<TState, TStateSchema>
    where TState : IJsonNodeState<TState>
    where TStateSchema : IJsonNodeStateSchema<TState, TStateSchema>
{
}

public interface IJsonNodeResponse<TState, TStateSchema, TResponse> :
    IResponse<TState, TStateSchema, TResponse>
    where TState : IJsonNodeState<TState>
    where TStateSchema : IJsonNodeStateSchema<TState, TStateSchema>
    where TResponse : IJsonNodeResponse<TState, TStateSchema, TResponse>
{
}
