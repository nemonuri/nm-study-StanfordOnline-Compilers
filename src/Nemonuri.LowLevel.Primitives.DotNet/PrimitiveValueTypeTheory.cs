
using static Nemonuri.LowLevel.Primitives.DotNet.UnmanagedTypeInfo;

namespace Nemonuri.LowLevel.Primitives.DotNet;

public static class PrimitiveValueTypeTheory
{
    public static readonly UnmanagedTypeInfo IntPtrInfo = Create<nint>();

    // Reference: https://learn.microsoft.com/en-us/dotnet/api/system.type.isprimitive?view=net-10.0#system-type-isprimitive
    // The primitive types are 
    //   Boolean, Byte, SByte, Int16, UInt16, 
    //   Int32, UInt32, Int64, UInt64, IntPtr, 
    //   UIntPtr, Char, Double, Single.

    private static readonly UnmanagedTypeInfo[] _typeInfos = [
        Create<bool>(), Create<byte>(), Create<sbyte>(), Create<short>(), Create<ushort>(),
        Create<int>(), Create<uint>(), Create<long>(), Create<ulong>(), IntPtrInfo,
        Create<nuint>(), Create<char>(), Create<double>(), Create<float>()
    ];

    private static readonly Dictionary<nint, int> _typeInfoMap = _typeInfos
        .Select(static (s, i) => (s, i))
        .ToDictionary(keySelector: static pair => pair.s.TypeHandleValue, elementSelector: static pair => pair.i);

    public static ref readonly UnmanagedTypeInfo GetInfo(RuntimeTypeHandle typeHandle)
    {
        try
        {
            return ref _typeInfos[_typeInfoMap[typeHandle.Value]];
        }
        catch (KeyNotFoundException e)
        {
            throw new ArgumentException
            (
                message: $"Type might be not primitive. {nameof(typeHandle)}={typeHandle.Value}, Type={Type.GetTypeFromHandle(typeHandle)?.ToString() ?? "<null>"}",
                innerException: e
            );
        }
        
    }
}
