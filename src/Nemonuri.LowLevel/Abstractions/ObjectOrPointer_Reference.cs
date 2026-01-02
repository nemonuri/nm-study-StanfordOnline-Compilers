
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
}
