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

    public static ref readonly ReadOnlyTypeBox<TDom, TSource> ReadOnlyBox<TDom, TSource>(ref readonly TSource source)
    {
        return ref Unsafe.As<TSource, ReadOnlyTypeBox<TDom, TSource>>(ref Unsafe.AsRef(in source));
    }

    public static ref readonly ReadOnlyTypeBox<TDomTo, TSource> ReadOnlyRebox<TDomFrom, TDomTo, TSource>
        (ref readonly ReadOnlyTypeBox<TDomFrom, TSource> boxedSource)
    {
        return ref Unsafe.As<ReadOnlyTypeBox<TDomFrom, TSource>, ReadOnlyTypeBox<TDomTo, TSource>>(ref Unsafe.AsRef(in boxedSource));
    }
}
