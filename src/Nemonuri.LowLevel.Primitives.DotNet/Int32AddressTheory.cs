
namespace Nemonuri.LowLevel.Primitives.DotNet;

public static class Int32AddressTheory
{
    public const int None = -1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNone(int val) => val == None;
}

