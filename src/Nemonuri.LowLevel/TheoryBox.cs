
namespace Nemonuri.LowLevel;

public struct TheoryDomain<TDom>{}

[StructLayout(LayoutKind.Sequential)]
public readonly struct TheoryBox<TDom, TSource>
{
    public readonly TSource Self;
}

public static class TheoryBox
{
    public static ref readonly TheoryBox<TDom, TSource> Box<TDom, TSource>(ref readonly TSource source) =>
        ref UnsafeReadOnly.As<TSource, TheoryBox<TDom, TSource>>(in source);

    public static ref readonly TheoryBox<TDomTo, TSource> ReadOnlyRebox<TDomFrom, TDomTo, TSource>
        (this ref readonly TheoryBox<TDomFrom, TSource> boxedSource) =>
        ref UnsafeReadOnly.As<TheoryBox<TDomFrom, TSource>, TheoryBox<TDomTo, TSource>>(in boxedSource);
    
    public static ref readonly TheoryBox<(TDom, TPush), TSource> Push<TDom, TSource, TPush>
        (this ref readonly TheoryBox<TDom, TSource> boxedSource, TheoryDomain<TPush> push = default)
    {
        return ref ReadOnlyRebox<TDom, (TDom, TPush), TSource>(in boxedSource);
    }

    public static ref readonly TheoryBox<TDom, TSource> Pop<TDom, TSource, TPop>
        (this ref readonly TheoryBox<(TDom, TPop), TSource> boxedSource)
    {
        return ref ReadOnlyRebox<(TDom, TPop), TDom, TSource>(in boxedSource);
    }
}
