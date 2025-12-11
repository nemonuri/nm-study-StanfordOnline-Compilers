using System.Text.Json.Nodes;
using Json.Schema;

namespace DscTool.Json;

public interface IJsonNodeState : IState
{
    JsonSchema? JsonSchema {get;}
}

public class JsonObjectDscComponent<
    TJsonNode, TPrecondition, 
    TState, TPostcondition
    > :
    IDscComponent<
        TJsonNode, TPrecondition, 
        TState, TPostcondition>
    where TJsonNode : JsonNode
    where TJsonNodePredicate : IJsonNodePredicate
    where TState : IJsonNodeState
{

}
