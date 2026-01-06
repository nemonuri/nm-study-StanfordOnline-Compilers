using System.Reflection;

namespace Nemonuri.LowLevel.Primitives.DotNet;

/// <summary>
/// Lazy-initializing CLR Type info
/// </summary>
public class TypeInfo
{
    internal TypeInfo(RuntimeTypeHandle runtimeTypeHandle)
    {
        RuntimeTypeHandle = runtimeTypeHandle;
    }

    public RuntimeTypeHandle RuntimeTypeHandle {get;}

    public System.Reflection.TypeInfo DotNetTypeInfo => field ??= Type.GetTypeFromHandle(RuntimeTypeHandle).GetTypeInfo();



    private bool? _isUnmanaged;
    public bool IsUnmanaged
    {
        get
        {
            if (!_isUnmanaged.HasValue)
            {
                _isUnmanaged = CalculateIsUnmanaged();
            }
            
            return _isUnmanaged.Value;
        }
    }

    private bool CalculateIsUnmanaged()
    {
        // Reference: https://github.com/dotnet/maintenance-packages/blob/main/src/System.Memory/src/System/SpanHelpers.cs

        var dti = DotNetTypeInfo;
        if (dti.IsPrimitive) {return true;}

        if (!dti.IsValueType) {return false;}

        Type? underlyingNullable = Nullable.GetUnderlyingType(dti);
        if (underlyingNullable != null)
        {
            return RuntimeTypeTheory.IsUnmanaged(underlyingNullable.TypeHandle);
        }

        if (dti.IsEnum) {return true;}

        foreach (FieldInfo fi in dti.DeclaredFields)
        {
            if (fi.IsStatic) { continue; }
            if (!RuntimeTypeTheory.IsUnmanaged(fi.FieldType.TypeHandle)) { return false; }
        }

        return true;
    }



}

internal static class TypeInfo<T>
{
    public static TypeInfo Instance {get;} = RuntimeTypeTheory.GetTypeInfo(typeof(T).TypeHandle);
}
