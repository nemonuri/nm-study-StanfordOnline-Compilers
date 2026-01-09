
using static Nemonuri.LowLevel.Primitives.DotNet.UnmanagedTypeInfo;
using static Nemonuri.LowLevel.Primitives.DotNet.Extensions.ImmutableArrayExtensions;

namespace Nemonuri.LowLevel.Primitives.DotNet;

public static partial class PrimitiveValueTypeTheory
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

    //public static bool IsPrimitive(RuntimeTypeHandle typeHandle) => _typeInfoMap.ContainsKey(typeHandle.Value);

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

public static partial class PrimitiveValueTypeTheory
{
    public static int IntPtrSize {get;} = System.IntPtr.Size;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static System.Type[] CreateDotNetTypeArray()
    {
        // Reference: https://learn.microsoft.com/en-us/dotnet/api/system.type.isprimitive?view=net-10.0#system-type-isprimitive
        // The primitive types are 
        //   Boolean, Byte, SByte, Int16, UInt16, 
        //   Int32, UInt32, Int64, UInt64, IntPtr, 
        //   UIntPtr, Char, Double, Single.

        // Type order is based on usage frequently. (subjective)
        // - More usage, less index.

        return 
        [
            typeof(int), typeof(bool), typeof(char), typeof(byte), typeof(nint),
            typeof(float), typeof(uint), typeof(long), typeof(double), typeof(nuint),
            typeof(ulong), typeof(short), typeof(ushort), typeof(sbyte)
        ];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int[] CreateDotNetSizeArray()
    {
        Debug.Assert( Unsafe.SizeOf<nint>() == IntPtrSize );
        Debug.Assert( Unsafe.SizeOf<nuint>() == IntPtrSize );

        return 
        [
            sizeof(int), sizeof(bool), sizeof(char), sizeof(byte), Unsafe.SizeOf<nint>(),
            sizeof(float), sizeof(uint), sizeof(long), sizeof(double), Unsafe.SizeOf<nuint>(),
            sizeof(ulong), sizeof(short), sizeof(ushort), sizeof(sbyte)
        ];
    }

    public static ImmutableArray<System.Type> DotNetTypes {get;} = ImmutableCollectionsMarshal.AsImmutableArray( CreateDotNetTypeArray() );

    public static ImmutableArray<int> Sizes {get;} = ImmutableCollectionsMarshal.AsImmutableArray( CreateDotNetSizeArray() );

    private static RuntimeTypeHandle ConvertDotNetTypeToRuntimeTypeHandle(System.Type type) => type.TypeHandle;
    public unsafe static ImmutableArray<RuntimeTypeHandle> RuntimeTypeHandles {get;} = DotNetTypes.ConvertAll(&ConvertDotNetTypeToRuntimeTypeHandle);
    
    private static nint ConvertRuntimeTypeHandleToRuntimeTypeId(RuntimeTypeHandle rth) => rth.Value;
    public unsafe static ImmutableArray<nint> RuntimeTypeIds {get;} = RuntimeTypeHandles.ConvertAll(&ConvertRuntimeTypeHandleToRuntimeTypeId);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf(RuntimeTypeHandle rth) => IndexOf(rth.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf(nint rid) => RuntimeTypeIds.IndexOf(rid);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPrimitiveValueType(RuntimeTypeHandle rth) => IsPrimitiveValueType(rth.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPrimitiveValueType(nint rid) => IndexOf(rid) >= 0;
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSizeOrZero(RuntimeTypeHandle rth) => GetSizeOrZero(rth.Value);

    public static int GetSizeOrZero(nint rid)
    {
        int index = IndexOf(rid);
        if (index < 0) { return 0; }
        Debug.Assert( index < Sizes.Length );

        int size = Sizes[index];
        Debug.Assert( size > 0 );
        return size;
    }
}
