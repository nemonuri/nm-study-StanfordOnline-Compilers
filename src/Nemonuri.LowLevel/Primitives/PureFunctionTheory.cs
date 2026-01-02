
namespace Nemonuri.LowLevel.Primitives;

public static class PureFunctionTheory
{
    public static T Identity<T>(T source) 
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
        => source;

    public static ref T Identity<T>(ref T location)
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
        => ref location;

    public static bool Tautology<T>(T source)
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
        => true;

    public static bool Tautology<T>(ref T source)
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
        => true;
    
    public static bool Falsum<T>(T source)
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
        => false;

    public static bool Falsum<T>(ref T source)
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
        => false;

}

