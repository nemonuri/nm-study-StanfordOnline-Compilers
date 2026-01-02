
namespace Nemonuri.LowLevel.Abstractions;

public readonly partial record struct ObjectOrPointer
{
    //--- reference-related deconstructors ---
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanDereference() => Object is ObjectOrPointerReference;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ObjectOrPointer DangerousDereference() => DangerousUnbox<ObjectOrPointerReference>().Dereference();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ObjectOrPointer GetSelfOrDereferenced() => CanDereference() ? DangerousDereference() : this;
    //---|

    //--- reference-related constructors ---
    public unsafe ObjectOrPointer FromManagedPointer<T>(ref T location, delegate*<ref T, bool> isFixedHint = default)
    {
        
    }
    //---|
}

internal readonly ref struct ManagedPointerHint
{
    public readonly 
#if NET8_0_OR_GREATER
    ref readonly byte 
#else
    ReadOnlySpan<byte>
#endif
    ManagedPointer;
    
    public readonly bool IsFixed;

#if NETSTANDARD2_1_OR_GREATER
    public ManagedPointerHint(ref readonly byte managedPointer, bool isFixed)
    {
        ManagedPointer = 
#if NET8_0_OR_GREATER
            ref managedPointer
#else
            MemoryMarshal.CreateReadOnlySpan<byte>(in managedPointer, 1)
#endif
        ;
        IsFixed = isFixed;
    }
#endif

}