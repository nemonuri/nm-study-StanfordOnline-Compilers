using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DscTool;

public struct TypeHint<TDom>{}

[StructLayout(LayoutKind.Sequential)]
public readonly struct ReadOnlyTypeBox<TDom, TSource>
{
    public readonly TSource Self;
}

[StructLayout(LayoutKind.Sequential)]
public struct TypeBox<TDom, TSource>
{
    public TSource Self;
}

public static class TypeBox
{
    public static ref TypeBox<TDom, TSource> Box<TDom, TSource>(ref TSource source)
    {
        return ref Unsafe.As<TSource, TypeBox<TDom, TSource>>(ref source);
    }

    public static ref readonly ReadOnlyTypeBox<TDom, TSource> ReadOnlyBox<TDom, TSource>(ref readonly TSource source) =>
        ref Unsafe.ReadOnlyAs<TSource, ReadOnlyTypeBox<TDom, TSource>>(in source);

    public static ref readonly ReadOnlyTypeBox<TDomTo, TSource> ReadOnlyRebox<TDomFrom, TDomTo, TSource>
        (this ref readonly ReadOnlyTypeBox<TDomFrom, TSource> boxedSource) =>
        ref Unsafe.ReadOnlyAs<ReadOnlyTypeBox<TDomFrom, TSource>, ReadOnlyTypeBox<TDomTo, TSource>>(in boxedSource);
    
    public static ref readonly ReadOnlyTypeBox<(TDom, TPush), TSource> Push<TDom, TSource, TPush>
        (this ref readonly ReadOnlyTypeBox<TDom, TSource> boxedSource, TypeHint<TPush> push = default)
    {
        return ref ReadOnlyRebox<TDom, (TDom, TPush), TSource>(in boxedSource);
    }

    public static ref readonly ReadOnlyTypeBox<TDom, TSource> Pop<TDom, TSource, TPop>
        (this ref readonly ReadOnlyTypeBox<(TDom, TPop), TSource> boxedSource)
    {
        return ref ReadOnlyRebox<(TDom, TPop), TDom, TSource>(in boxedSource);
    }
}
