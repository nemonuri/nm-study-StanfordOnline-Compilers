
namespace Nemonuri.LowLevel.Runtime;

// Copy from: https://github.com/CommunityToolkit/dotnet/blob/main/src/CommunityToolkit.HighPerformance/Helpers/ObjectMarshal.cs
[StructLayout(LayoutKind.Explicit)]
public sealed class RawObjectData
{
    [FieldOffset(0)]
    public byte Data;

    private RawObjectData() { throw new NotSupportedException(); }
}