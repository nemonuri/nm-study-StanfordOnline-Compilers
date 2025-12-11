
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

public enum DscStateKind
{
    Unknown = 0,
    Desired = 1,
    Current = 2
}

public enum DscResponseKind
{
    Unknown = 0,
    Test = 1,
    Edit = 2
}
