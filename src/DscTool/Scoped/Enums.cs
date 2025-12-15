
using System.Runtime.InteropServices;

namespace DscTool.Scoped;

[StructLayout(LayoutKind.Explicit)]
public readonly record struct LiftErrorInfo
(
    [field: FieldOffset(0)] LiftErrorKind ErrorKind,
    [field: FieldOffset(1)] HoareTripleErrorKind InnerErrorKind
);

public enum LiftErrorKind : byte
{
    Unknown = 0,
    ConditionLifterInnerError = 1,
    LiftedPreconditionIsNotSufficient = 2,
    EmbedderPreconditionIsNotSufficient = 3

}
