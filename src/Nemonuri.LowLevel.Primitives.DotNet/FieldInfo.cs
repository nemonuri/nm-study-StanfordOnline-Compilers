
using System.Reflection;

namespace Nemonuri.LowLevel.Primitives.DotNet;

[StructLayout(LayoutKind.Sequential)]
public struct FieldInfo
{
    public readonly int Size = Unsafe.SizeOf<FieldInfo>();

    public const int PreviousFieldAddressNone = -1;
    public const int ExplicitFieldOffsetNone = -1;

    public FieldInfo(int address, RuntimeFieldAndTypeHandle runtimeFieldHandle, int previousFieldAddress = PreviousFieldAddressNone)
    {
        Address = address;
        RuntimeFieldAndTypeHandle = runtimeFieldHandle;
        PreviousFieldAddressOrNone = previousFieldAddress;
        _flags = 0;
    }

    #region Fields

    public readonly int Address;

    public readonly RuntimeFieldAndTypeHandle RuntimeFieldAndTypeHandle;

    public readonly int PreviousFieldAddressOrNone;

    private uint _flags;

    //--- well-known properties ---
    private int _fieldTypeAddress;

    private int _declaringTypeAddress;

    private FieldAttributes _fieldAttributes;
    //---|  

    private int _explicitFieldOffsetOrNone;

    #endregion Fields

    public readonly RuntimeFieldHandle RuntimeFieldHandle => RuntimeFieldAndTypeHandle.RuntimeFieldHandle;

    public readonly RuntimeTypeHandle DeclaringTypeHandle => RuntimeFieldAndTypeHandle.DeclaringTypeHandle;

    public readonly bool TryGetPreviousFieldAddress(out int address) => (address = PreviousFieldAddressOrNone) is not PreviousFieldAddressNone;

    private const uint WellKnownPropertiesAssignedMask = 1 << 0;

    private void AssignWellKnownPropertiesIfNeeded()
    {
        if ((_flags & WellKnownPropertiesAssignedMask) != 0) {return;}

        System.Reflection.FieldInfo fieldInfo = RuntimeFieldAndTypeHandle.DotNetFieldInfo;
        RuntimeTypeHandle fieldTypeHandle = fieldInfo.FieldType.TypeHandle;
        _fieldTypeAddress = RuntimeTypeTheory.GetTypeInfoAddress(fieldTypeHandle);
        _declaringTypeAddress = RuntimeTypeTheory.GetTypeInfoAddress(DeclaringTypeHandle);
        _fieldAttributes = fieldInfo.Attributes;

        _flags |= WellKnownPropertiesAssignedMask;
    }

    public int FieldTypeAddress { get { AssignWellKnownPropertiesIfNeeded(); return _fieldTypeAddress; } }

    public int DeclaringTypeAddress { get { AssignWellKnownPropertiesIfNeeded(); return _declaringTypeAddress; } }

    public RuntimeTypeHandle FieldTypeHandle => RuntimeTypeTheory.TypeInfos[FieldTypeAddress].RuntimeTypeHandle;

    public FieldAttributes FieldAttributes { get { AssignWellKnownPropertiesIfNeeded(); return _fieldAttributes; } }


    private const uint ExplicitFieldOffsetOrNoneAssignedMask = 1 << 1;

    public bool TryGetExplicitFieldOffset(out int offset)
    {
        if ((_flags & ExplicitFieldOffsetOrNoneAssignedMask) == 0)
        {
            _explicitFieldOffsetOrNone = ExplicitFieldOffsetNone;
            if (RuntimeTypeTheory.TypeInfos[FieldTypeAddress].LayoutKind != LayoutKind.Explicit) { goto FlagAndExit; }
            if (RuntimeFieldAndTypeHandle.DotNetFieldInfo.GetCustomAttribute<FieldOffsetAttribute>() is not { } fieldOffset) { goto FlagAndExit; }

            _explicitFieldOffsetOrNone = fieldOffset.Value;

        FlagAndExit:
            _flags |= ExplicitFieldOffsetOrNoneAssignedMask;
        }

        return (offset = _explicitFieldOffsetOrNone) is not ExplicitFieldOffsetNone;
    }

}
