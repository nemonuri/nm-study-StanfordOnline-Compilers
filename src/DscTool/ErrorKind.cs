
namespace DscTool;

public enum HoareTripleErrorKind
{
    Unknown = 0,
    PreCondition = 1,
    PostCondition = 2
}

public enum HoareTripleLiftCommandErrorKind
{
    Unknown = 0,
    PreConditionLiftPreCondition = 1,
    PreConditionLiftPostCondition = 2,
    PredicateLiftPostConditionIsNull = 3
}