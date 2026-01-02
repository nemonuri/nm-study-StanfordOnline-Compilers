
namespace Nemonuri.LowLevel.Primitives;

public static class UnmanagedValueTheory
{
    public static ref TTo BitCastOrUndefined<TFrom, TTo>(ref TFrom from)
        where TFrom : unmanaged
        where TTo : unmanaged
    {
        if (!RuntimeTypeTheory.AreSizeEqual<TFrom, TTo>()) { return ref RuntimePointerTheory.UndefinedRef<TTo>(); }
        return ref Unsafe.As<TFrom, TTo>(ref from);
    }

    public static ref TTo BitCastOrUndefined<TFrom, TTo>(ref TFrom from, RuntimeTypeHandle typeGuard)
        where TFrom : unmanaged
        where TTo : unmanaged
    {
        if (!RuntimeTypeTheory.AreEqual<TTo>(typeGuard)) { return ref RuntimePointerTheory.UndefinedRef<TTo>(); }
        return ref BitCastOrUndefined<TFrom, TTo>(ref from);
    }
    
}
