
using System.Reflection;

namespace Nemonuri.LowLevel.Primitives.DotNet;

[StructLayout(LayoutKind.Sequential)]
internal partial struct FieldInfo
{

    public readonly int Size = Unsafe.SizeOf<FieldInfo>();


    public FieldInfo(int address, RuntimeFieldAndTypeHandle runtimeFieldHandle, int previousFieldAddress = Int32AddressTheory.None)
    {
        Address = address;
        RuntimeFieldAndTypeHandle = runtimeFieldHandle;
        PreviousFieldAddressOrNone = previousFieldAddress;
        _flags = default;
    }

    #region Fields

    public readonly int Address;

    public readonly RuntimeFieldAndTypeHandle RuntimeFieldAndTypeHandle;

    public readonly int PreviousFieldAddressOrNone;

    private ThreadSafeFlags _flags;

    //--- well-known properties ---
    private bool _isFieldTypePrimitive;

    // If field type is primitive, this field is address of primitive type.
    // Else, this field is address of runtime type.
    private int _fieldTypeAddress;

    private int _declaringTypeAddress;

    private FieldAttributes _fieldAttributes;
    //---|  

    //--- ordinal properties ---
    private int _ordinal;
    //---|

    //--- offset properties ---
    private int _offsetOrNone;

    //---|

    #endregion Fields

    public readonly RuntimeFieldHandle RuntimeFieldHandle => RuntimeFieldAndTypeHandle.RuntimeFieldHandle;

    public readonly RuntimeTypeHandle DeclaringTypeHandle => RuntimeFieldAndTypeHandle.DeclaringTypeHandle;

    public readonly bool TryGetPreviousFieldAddress(out int address) => (address = PreviousFieldAddressOrNone) is not Int32AddressTheory.None;

    //--- well-known properties ---

    private void AssignWellKnownPropertiesIfNeeded()
    {
        if (_flags.HasFlags(Flags.WellKnownPropertiesAssigned)) {return;}

        System.Reflection.FieldInfo fieldInfo = RuntimeFieldAndTypeHandle.DotNetFieldInfo;
        RuntimeTypeHandle fieldTypeHandle = fieldInfo.FieldType.TypeHandle;

        int addressCandidate = PrimitiveValueTypeTheory.IndexOf(fieldTypeHandle);
        if (addressCandidate >= 0)
        {
            _isFieldTypePrimitive = true;
            _fieldTypeAddress = addressCandidate;
        }
        else
        {
            _isFieldTypePrimitive = false;
            _fieldTypeAddress = RuntimeTypeTheory.GetOrAddAddress(fieldTypeHandle);
        }

        _declaringTypeAddress = RuntimeTypeTheory.GetOrAddAddress(DeclaringTypeHandle);
        _fieldAttributes = fieldInfo.Attributes;

        _flags.AddFlags(Flags.WellKnownPropertiesAssigned);
    }

    public bool IsFieldTypePrimitive { get { AssignWellKnownPropertiesIfNeeded(); return _isFieldTypePrimitive; } }

    public int FieldTypeAddress { get { AssignWellKnownPropertiesIfNeeded(); return _fieldTypeAddress; } }

    public int DeclaringTypeAddress { get { AssignWellKnownPropertiesIfNeeded(); return _declaringTypeAddress; } }

    public FieldAttributes FieldAttributes { get { AssignWellKnownPropertiesIfNeeded(); return _fieldAttributes; } }

    public ref readonly TypeInfo DeclaringTypeInfo => ref RuntimeTypeTheory.TypeInfos[DeclaringTypeAddress];

    //public bool IsFieldTypeValueType => IsFieldTypePrimitive || RuntimeTypeTheory.TypeInfos[FieldTypeAddress].IsValueType;

    public bool HasStableLayoutAsLeaf
    {
        get
        {
            if (IsFieldTypePrimitive) {return true;}
            ref readonly TypeInfo fieldTypeInfo = ref RuntimeTypeTheory.TypeInfos[FieldTypeAddress];
            return !fieldTypeInfo.IsValueType || fieldTypeInfo.HasStableLayoutAsContainer;
        }
    }

    //---|

    //--- ordinal properties ---

    public int Ordinal
    {
        get
        {
            if (!_flags.HasFlags(Flags.OrdinalPropertiesAssigned))
            {
                if (TryGetPreviousFieldAddress(out int prevAddress))
                {
                    _ordinal = RuntimeFieldTheory.FieldInfos[prevAddress].Ordinal + 1;
                }
                else
                {
                    _ordinal = 0;
                }

                _flags.AddFlags(Flags.OrdinalPropertiesAssigned);
            }

            return _ordinal;
        }
    }

    //---|

    //--- offset properties ---

    public int StableLeafSizeOrZero
    {
        get
        {
            int leafSize;
            if (IsFieldTypePrimitive)
            {
                leafSize = PrimitiveValueTypeTheory.Sizes[FieldTypeAddress];
            }
            else
            {
                ref readonly TypeInfo ftInfo = ref RuntimeTypeTheory.TypeInfos[FieldTypeAddress];
                if (ftInfo.IsValueType)
                {
                    leafSize = ftInfo.StableContainerSizeOrZero;
                }
                else
                {
                    leafSize = ftInfo.StableLeafSizeOrZero;
                }
            }
            return leafSize;
        }
    }

    public int OffsetOrNone
    {
        get
        {
            if (!_flags.HasFlags(Flags.OffsetPropertiesAssigned))
            {
                ref readonly TypeInfo dtInfo = ref DeclaringTypeInfo;
                if (!dtInfo.HasStableLayoutAsContainer)
                {
                    _offsetOrNone = Int32AddressTheory.None;
                    goto AddFlagsAndExit;
                }

                if (dtInfo.IsExplicitLayout)
                {
                    FieldOffsetAttribute? fieldOffsetAttribute = RuntimeFieldAndTypeHandle.DotNetFieldInfo.GetCustomAttribute<FieldOffsetAttribute>();
                    if (fieldOffsetAttribute is null) {throw new ArgumentNullException(nameof(fieldOffsetAttribute));}
                    _offsetOrNone = fieldOffsetAttribute.Value;
                }
                else if (dtInfo.IsSequentialLayout)
                {
                    if (TryGetPreviousFieldAddress(out int prevAddress))
                    {
                        int prevAddressOffset = RuntimeFieldTheory.FieldInfos[prevAddress].OffsetOrNone;
                        if (Int32AddressTheory.IsNone(prevAddressOffset))
                        {
                            throw new ArgumentNullException(nameof(prevAddressOffset));
                        }

                        int leafSize = StableLeafSizeOrZero;
                        if (leafSize == 0)
                        {
                            throw new ArgumentOutOfRangeException(nameof(leafSize));
                        }

                        int clPack = dtInfo.ClassLayoutPack;

                        //System.Numerics.BitOperations
                    }
                    else
                    {
                        _offsetOrNone = 0;
                    }
                }
                else
                {
                    _offsetOrNone = Int32AddressTheory.None;
                }

            AddFlagsAndExit:
                _flags.AddFlags(Flags.OffsetPropertiesAssigned);
            }

            return _offsetOrNone;
        }
    }

    //---|



}
