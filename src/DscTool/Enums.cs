
using System.Runtime.InteropServices;

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

[Flags]
public enum TreeWalkErrorKind : byte
{
    Unknown = 0,
    InvalidRoot = 1 << 0,
    InvalidChild = 1 << 1,
    OutOfDepth = 1 << 2,
    OutOfWidth = 1 << 3,
    OutOfDepthOrWidth = OutOfDepth | OutOfWidth
}

[Flags]
public enum MorphErrorSide : byte
{
    Unknown = 0,
    Source = 1,
    Target = 2,
    Both = 3
}

[StructLayout(LayoutKind.Explicit)]
public readonly record struct TreeMorphErrorKind
(
    [field: FieldOffset(0)] TreeWalkErrorKind TreeWalkErrorKind,
    [field: FieldOffset(1)] MorphErrorSide MorphErrorSide
);

public enum IndexPathFindErrorKind
{
    Unknown = 0,
    IsOutOfWidthAt = 1,
    IsOutOfDepthAt = 2
}



