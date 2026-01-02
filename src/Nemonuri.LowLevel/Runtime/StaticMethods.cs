
namespace Nemonuri.LowLevel.Runtime;

internal static class StaticMethods
{
    public static ref T Identity<T>(ref T source) => ref source;

    public static bool AreEquivalent<T>(in T? left, in T? right)
        where T : IEquatable<T>
    {
        if (!UnsafeReadOnly.AreNotNullRef(in left, in right)) {return false;}
        if (UnsafeReadOnly.AreSameRef(in left, in right)) {return true;}

        return left?.Equals(right) ?? false;
    }
}
