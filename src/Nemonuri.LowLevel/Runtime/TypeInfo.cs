#if !NETSTANDARD2_1_OR_GREATER
using System.Reflection;

namespace Nemonuri.LowLevel.Runtime;

internal static class TypeInfo<T>
{
    /// <summary>
    /// Indicates whether <typeparamref name="T"/> does not respect the <see langword="unmanaged"/> constraint.
    /// </summary>
    public static readonly bool IsReferenceOrContainsReferences = TypeInfo.IsReferenceOrContainsReferences(typeof(T));
}

internal static class TypeInfo
{
    public static bool IsReferenceOrContainsReferences(Type type)
    {
        // Common case, for primitive types
        if (type.IsPrimitive)
        {
            return false;
        }

        // Explicitly check for pointer types first
        if (type.IsPointer)
        {
            return false;
        }

        // Check for value types (this has to be after checking for pointers)
        if (!type.IsValueType)
        {
            return true;
        }

        // Check if the type is Nullable<T>
        if (Nullable.GetUnderlyingType(type) is Type nullableType)
        {
            type = nullableType;
        }

        if (type.IsEnum)
        {
            return false;
        }

        // Complex struct, recursively inspect all fields
        foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (IsReferenceOrContainsReferences(field.FieldType))
            {
                return true;
            }
        }

        return false;
    }
}
#endif