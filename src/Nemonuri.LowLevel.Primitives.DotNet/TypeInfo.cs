
using System.Reflection;
using static Nemonuri.LowLevel.Primitives.DotNet.Extensions.RuntimeTypeHandleExtensions;

namespace Nemonuri.LowLevel.Primitives.DotNet;

/// <summary>
/// Lazy-initializing CLR Type info
/// </summary>
internal partial struct TypeInfo
{
    private const int None = -1;

    internal TypeInfo(int address, RuntimeTypeHandle runtimeTypeHandle)
    {
        Address = address;
        RuntimeTypeHandle = runtimeTypeHandle;
        _flags = default;
    }

    #region Fields

    public readonly int Address;

    public readonly RuntimeTypeHandle RuntimeTypeHandle;

    private ThreadSafeFlags _flags;

    //--- Well-known properties ---
    private bool _isValueType;

    private TypeAttributes _typeAttributes;
    //---|

    //--- Enum properties ---
    private RuntimeTypeHandle _enumUnderlyingRuntimeTypeHandleOrNull;
    //---|

    //--- Nullable properties ---
    private RuntimeTypeHandle _nullableUnderlyingRuntimeTypeHandleOrNull;
    //---|

    //--- Class layout properties ---
    private int _classLayoutPack;

    private int _classLayoutSize;
    //---|

    //--- Instance fields properties ---
    private int _instanceFieldListStartAddress;

    private int _instanceFieldListCount;
    //---|

    //--- Stable layout properties ---
    private bool _hasStableLayoutAsContainer;
    //---|

    //--- Size properties ---
    private int _stableContainerSizeOrZero;
    
    private int _stableLeafSizeOrZero;
    //---|

    private int _isUnmanaged;

    private bool _isLayoutStableValueType;

    

    #endregion Fields

    public readonly Type DotNetType => this.RuntimeTypeHandle.GetDotNetType();

    //--- Well-Known properties ---
    public bool IsValueType { get { AssignWellKnownPropertiesIfNeeded(); return _isValueType; } }

    public TypeAttributes TypeAttributes { get { AssignWellKnownPropertiesIfNeeded(); return _typeAttributes; } }

    private void AssignWellKnownPropertiesIfNeeded()
    {
        if (_flags.HasFlags(Flags.WellKnownPropertiesAssigned)) {return;}

        var dt = DotNetType;
        _isValueType = dt.IsValueType;
        _typeAttributes = dt.Attributes;

        _flags.AddFlags(Flags.WellKnownPropertiesAssigned);
    }
    //---|

    //--- Enum properties ---
    public RuntimeTypeHandle EnumUnderlyingRuntimeTypeHandleOrNull
    {
        get
        {
            if (!_flags.HasFlags(Flags.EnumPropertiesAssigned))
            {
                var dt = DotNetType;
                if (dt.IsEnum)
                {
                    _enumUnderlyingRuntimeTypeHandleOrNull = dt.GetEnumUnderlyingType().TypeHandle;
                }
                else
                {
                    _enumUnderlyingRuntimeTypeHandleOrNull = default;
                }

                _flags.AddFlags(Flags.EnumPropertiesAssigned);
            }

            return _enumUnderlyingRuntimeTypeHandleOrNull;
        }
    }

    public bool IsEnum => !EnumUnderlyingRuntimeTypeHandleOrNull.IsNull;
    //---|

    //--- Nullable properties ---
    public RuntimeTypeHandle NullableUnderlyingRuntimeTypeHandleOrNull
    {
        get
        {
            if (!_flags.HasFlags(Flags.NullablePropertiesAssigned))
            {
                Type? ut = Nullable.GetUnderlyingType(DotNetType);
                _nullableUnderlyingRuntimeTypeHandleOrNull = ut?.TypeHandle ?? default;

                _flags.AddFlags(Flags.NullablePropertiesAssigned);
            }

            return _nullableUnderlyingRuntimeTypeHandleOrNull;
        }
    }

    public bool IsNullableValueType => !NullableUnderlyingRuntimeTypeHandleOrNull.IsNull;
    //---|

    //--- Class layout properties ---
    private void AssignClassLayoutPropertiesIfNeeded()
    {
        if (_flags.HasFlags(Flags.ClassLayoutPropertiesAssigned)) {return;}

        var dt = DotNetType;
        if (dt.StructLayoutAttribute is { Value: LayoutKind.Sequential or LayoutKind.Explicit } sl)
        {
            _classLayoutPack = sl.Pack;
            _classLayoutSize = sl.Size;
        }
        else
        {
            _classLayoutPack = 0;
            _classLayoutSize = 0;
        }

        _flags.AddFlags(Flags.ClassLayoutPropertiesAssigned);
    }

    public int ClassLayoutPack { get { AssignClassLayoutPropertiesIfNeeded(); return _classLayoutPack; } }

    public int ClassLayoutSize { get { AssignClassLayoutPropertiesIfNeeded(); return _classLayoutSize; } }

    public bool IsAutoLayout => (TypeAttributes | TypeAttributes.LayoutMask) == TypeAttributes.AutoLayout;

    public bool IsSequentialLayout => (TypeAttributes | TypeAttributes.LayoutMask) == TypeAttributes.SequentialLayout;

    public bool IsExplicitLayout => (TypeAttributes | TypeAttributes.LayoutMask) == TypeAttributes.ExplicitLayout;

    public bool IsSequentialOrExplicitLayout => (TypeAttributes | TypeAttributes.LayoutMask) != TypeAttributes.AutoLayout;
    //---|

    //--- Instance fields properties ---
    private void AssignInstanceFieldsPropertiesIfNeeded()
    {
        if (_flags.HasFlags(Flags.InstanceFieldsPropertiesAssigned)) {return;}

        var dt = DotNetType;
        System.Reflection.FieldInfo[] fis = dt.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (fis.Length == 0) { goto AddFlagsAndExit; }

        Span<RuntimeFieldHandle> rfhs = stackalloc RuntimeFieldHandle[fis.Length];
        for (int i = 0; i < rfhs.Length; i++)
        {
            rfhs[i] = fis[i].FieldHandle;
        }

        _instanceFieldListStartAddress = RuntimeFieldTheory.AddFields(RuntimeTypeHandle, rfhs);
        _instanceFieldListCount = rfhs.Length;

        Span<FieldInfo> slicedFieldInfos = RuntimeFieldTheory.FieldInfos.Slice(_instanceFieldListStartAddress, _instanceFieldListCount);
        for (int i = 0; i < slicedFieldInfos.Length; i++)
        {
            int curAddress = _instanceFieldListStartAddress + i;
            int prevAddressOrNone = (i == 0) ? Int32AddressTheory.None : (curAddress - 1);
            slicedFieldInfos[i] = new(curAddress, new(rfhs[i], RuntimeTypeHandle), prevAddressOrNone);
        }

    AddFlagsAndExit:
        _flags.AddFlags(Flags.InstanceFieldsPropertiesAssigned);
    }

    public ReadOnlySpan<FieldInfo> InstanceFieldInfos
    {
        get
        {
            AssignInstanceFieldsPropertiesIfNeeded();
            return RuntimeFieldTheory.FieldInfos.Slice(_instanceFieldListStartAddress, _instanceFieldListCount);
        }
    }
    //---|

    //--- Stable layout properties ---
    public bool HasStableLayoutAsContainer
    {
        get
        {
            if (!_flags.HasFlags(Flags.StableLayoutPropertiesAssigned))
            {
                if (IsAutoLayout) {return false;}
                foreach (FieldInfo fi in InstanceFieldInfos)
                {
                    if (!fi.HasStableLayoutAsLeaf) 
                    { 
                        _hasStableLayoutAsContainer = false;
                        goto AddFlagsAndExit; 
                    }
                }
                _hasStableLayoutAsContainer = true;
            
            AddFlagsAndExit:
                _flags.AddFlags(Flags.StableLayoutPropertiesAssigned);
            }

            return _hasStableLayoutAsContainer;
        }
    }
    //---|

    //--- Size properties ---
    public void AssignStablePropertiesIfNeeded()
    {
        if (!_flags.HasFlags(Flags.SizePropertiesAssigned)) {return;}

        if (HasStableLayoutAsContainer)
        {
            throw new NotImplementedException();
        }
        else
        {
            _stableContainerSizeOrZero = 0;
            _stableLeafSizeOrZero = IsValueType ? 0 : PrimitiveValueTypeTheory.IntPtrSize;
        }

        _flags.AddFlags(Flags.SizePropertiesAssigned);
    }

    public int StableContainerSizeOrZero { get { AssignStablePropertiesIfNeeded(); return _stableContainerSizeOrZero; } }

    public int StableLeafSizeOrZero { get { AssignStablePropertiesIfNeeded(); return _stableLeafSizeOrZero; } }
    //---|



    private nint[] ValueTypeFieldOffsetsCore
    {
        get
        {
            if (field != null) {return field;}
            if (!IsValueType) { return field = []; }

            Debug.WriteLine($"{nameof(ValueTypeFieldOffsetsCore)}: Type={DotNetTypeInfo.AssemblyQualifiedName}");

            var fis = InstanceFieldInfos;
            int length = fis.Length;
            field = new nint[fis.Length];

            for (int i = 0; i < length; i++)
            {
                // Why 'EditorBrowsableState.Never'?
                field[i] = Marshal.OffsetOf(DotNetTypeInfo, fis[i].Name);
                Debug.WriteLine($"Index={i}, FieldName={fis[i].Name}, FieldType={fis[i].FieldType.FullName}, Offset={field[i]}");
            }

            return field;
        }
    }
    public ReadOnlySpan<nint> ValueTypeFieldOffsets => ValueTypeFieldOffsetsCore;


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

        foreach (System.Reflection.FieldInfo fi in InstanceFieldInfos)
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

        ReadOnlySpan<System.Reflection.FieldInfo> fieldInfos = InstanceFieldInfos;
        if (fieldInfos.IsEmpty)
        {
            // Size shall larger than zero.
            int defaultSize = 1;
            return Math.Max(defaultSize, desiredSize);
        }
        ReadOnlySpan<nint> fieldOffsets = ValueTypeFieldOffsets;

        if (layoutKind == LayoutKind.Sequential)
        {
            // If layoutkind is sequential, just check last element.
            int fiIndex = fieldInfos.Length - 1;
            System.Reflection.FieldInfo fi = fieldInfos[fiIndex];
            nint fiOffset = fieldOffsets[fiIndex];
            int fiSize = RuntimeTypeTheory.GetSizeOrZero(fi.FieldType.TypeHandle);

            int defaultSize = (int)fiOffset + fiSize;
            return Math.Max(defaultSize, desiredSize);
        }
        else
        {
            // else, check all elements.
            int defaultSizeCandidate = 1;

            for (int fiIndex = 0; fiIndex < fieldInfos.Length; fiIndex++)
            {
                System.Reflection.FieldInfo fi = fieldInfos[fiIndex];
                nint fiOffset = fieldOffsets[fiIndex];
                int fiSize = RuntimeTypeTheory.GetSizeOrZero(fi.FieldType.TypeHandle);

                defaultSizeCandidate = Math.Max(defaultSizeCandidate, (int)fiOffset + fiSize);
            }

            return Math.Max(defaultSizeCandidate, desiredSize);
        }
    }


}
