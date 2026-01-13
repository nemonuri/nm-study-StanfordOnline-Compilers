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

[StructLayout(LayoutKind.Explicit)]
public struct TTT
{
    [FieldOffset(0)]
    public object? Obj;

    [FieldOffset(1)]
    public object? Obj2;

    public TTT(object? obj, object? obj2)
    {
        Obj = obj;
        Obj2 = obj2;
    }
}
