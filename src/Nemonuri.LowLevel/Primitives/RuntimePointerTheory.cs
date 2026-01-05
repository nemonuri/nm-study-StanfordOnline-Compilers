
namespace Nemonuri.LowLevel.Primitives;

public static class RuntimePointerTheory
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static void* UndefinedPointer() => Unsafe.AsPointer(ref DotNet.ByRefTheory.UndefinedRef());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static bool IsUndefinedPointer(void* pointer) => pointer == UndefinedPointer();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static bool IsUndefinedOrNullPointer(void* pointer) => IsUndefinedPointer(pointer) || pointer == null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static T DangerousDereference<T>(void* pointer) => Unsafe.As<byte, T>(ref Unsafe.AsRef<byte>(pointer));
}
