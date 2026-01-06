using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using Polyfills;

namespace Nemonuri.LowLevel.Primitives.DotNet;

/// <summary>
/// Lazy-initializing CLR Type info
/// </summary>
public class TypeInfo
{
    internal TypeInfo(RuntimeTypeHandle runtimeTypeHandle)
    {
        RuntimeTypeHandle = runtimeTypeHandle;
        _flags = 0;
    }

    public RuntimeTypeHandle RuntimeTypeHandle {get;}

    public System.Reflection.TypeInfo DotNetTypeInfo => field ??= Type.GetTypeFromHandle(RuntimeTypeHandle).GetTypeInfo();

    private uint _flags;

    //--- Well-Known properties ---
    private const uint WellKnownPropertiesAssignedMask = 1 << 0;

    public bool IsPrimitive
    {
        get { AssignWellKnownPropertiesIfNeeded(); return field; }
        private set;
    }

    public bool IsValueType
    {
        get { AssignWellKnownPropertiesIfNeeded(); return field; }
        private set;
    }

    [MemberNotNullWhen(true, [nameof(EnumUnderlyingType)])]
    public bool IsEnum
    {
        get { AssignWellKnownPropertiesIfNeeded(); return field; }
        private set;
    }

    private void AssignWellKnownPropertiesIfNeeded()
    {
        if ((_flags & WellKnownPropertiesAssignedMask) != 0) {return;}

        var dti = DotNetTypeInfo;
        IsPrimitive = dti.IsPrimitive;
        IsValueType = dti.IsValueType;
        IsEnum = dti.IsEnum;

        _flags |= WellKnownPropertiesAssignedMask;
    }
    //---|

    public Type? EnumUnderlyingType => IsEnum ? (field ??= DotNetTypeInfo.GetEnumUnderlyingType()) : null;

    private const uint StructLayoutAttributeAssignedMask = 1 << 1;
    public StructLayoutAttribute? StructLayoutAttribute
    {
        get
        {
            if ((_flags & StructLayoutAttributeAssignedMask) != 0) {return field;}

            if (!IsValueType) { goto FlagAndReturn; }
            field = DotNetTypeInfo.StructLayoutAttribute;

        FlagAndReturn:
            _flags |= StructLayoutAttributeAssignedMask;
            return field;
        }
    }

    private const uint NullableUnderlyingTypeAssignedMask = 1 << 2;
    public Type? NullableUnderlyingType
    {
        get
        {
            if ((_flags & NullableUnderlyingTypeAssignedMask) != 0) {return field;}

            if (!IsValueType) { goto FlagAndReturn; }
            field = Nullable.GetUnderlyingType(DotNetTypeInfo);

        FlagAndReturn:
            _flags |= NullableUnderlyingTypeAssignedMask;
            return field;
        }
    }

    [MemberNotNullWhen(true, [nameof(NullableUnderlyingType)])]
    public bool IsNullableValueType => NullableUnderlyingType is not null;


    private FieldInfo[] InstanceFieldInfosCore => field ??= DotNetTypeInfo.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    public ReadOnlySpan<FieldInfo> InstanceFieldInfos => InstanceFieldInfosCore;

    private nint[] ValueTypeFieldOffsetsCore
    {
        get
        {
            if (field != null) {return field;}
            if (!IsValueType) { return field = []; }

            var fis = InstanceFieldInfos;
            int length = fis.Length;
            field = new nint[fis.Length];

            // Activator.CreateInstance
            // - 'type' argument should have public parameterless constructor.
            //   - Fortunately, all value type has public parameterless constructor.
            // - If 'type' is Nullable<T>, it returns `null`.
            object sampleInstance = Activator.CreateInstance(DotNetTypeInfo)
            for (int i = 0; i < length; i++)
            {
                var tr = TypedReference.MakeTypedReference(sampleInstance, [fis[i]]);
                tr.
                //ref object sdfsf = ref __refvalue(tr);
            }
        }
        
    }


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

        foreach (FieldInfo fi in InstanceFieldInfos)
        {
            Debug.Assert( !fi.IsStatic );
            if (!RuntimeTypeTheory.IsUnmanaged(fi.FieldType.TypeHandle)) { return false; }
        }

        return true;
    }

    private int _size = 0;
    public int Size
    {
        get
        {
            if (_size <= 0)
            {
                _size = CalculateSize();
            }

            return _size;
        }
    }

    private int CalculateSize()
    {
        var dti = DotNetTypeInfo;
        if (!IsValueType) { return PrimitiveValueTypeTheory.IntPtrInfo.Size; }

        if (IsPrimitive)
        {
            return PrimitiveValueTypeTheory.GetInfo(RuntimeTypeHandle).Size;
        }

        if (IsEnum)
        {
            return PrimitiveValueTypeTheory.GetInfo(EnumUnderlyingType.TypeHandle).Size;
        }

        //--- Check struct layout attribute ---

        // Reference: https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.structlayoutattribute.size?view=net-10.0#system-runtime-interopservices-structlayoutattribute-size
        // - This field must be equal or greater than the total size, in bytes, of the members of the class or structure.
        int desiredSize = 0;

        // Reference: https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.structlayoutattribute?view=net-10.0
        // - C#, Visual Basic, and C++ compilers apply the Sequential layout value to structures by default.
        LayoutKind layoutKind = LayoutKind.Sequential;

        if (StructLayoutAttribute is { } structLayoutInfo)
        {
            desiredSize = structLayoutInfo.Size;
            layoutKind = structLayoutInfo.Value;
        }

        //---|

        foreach (FieldInfo fi in InstanceFieldInfos)
        {
            
        }
    }

    public struct ASDF
    {
        private ASDF() {}
    }


}

internal static class TypeInfo<T>
{
    public static TypeInfo Instance {get;} = RuntimeTypeTheory.GetTypeInfo(typeof(T).TypeHandle);
}
