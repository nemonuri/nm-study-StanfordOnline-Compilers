namespace Nemonuri.LowLevel.Primitives.DotNet;

public static class VoidTypeTheory
{
    public static Type DotNetType {get;} = typeof(void);

    public static RuntimeTypeHandle RuntimeTypeHandle {get;} = DotNetType.TypeHandle;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsVoid(RuntimeTypeHandle rth) => IsVoid(rth.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsVoid(nint rid) => rid == DotNetType.TypeHandle.Value;
}
