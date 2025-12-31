
using CommunityToolkit.HighPerformance;

namespace Nemonuri.LowLevel;

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct GoPointer<T> : IRefBox<T>
    where T : struct
{
    public readonly Box<T>? Boxed;
    public readonly void* Unmanaged;

    public GoPointer(Box<T> boxed)
    {
        Boxed = boxed;
        Unmanaged = null;
    }

    public GoPointer(ref T unmanaged)
    {
        Boxed = null;
        Unmanaged = Unsafe.AsPointer(in unmanaged);
    }

    public bool IsNull => Boxed is null && Unmanaged == null;

    public ref T Value
    {
        get
        {
            if (Unmanaged == null) { return ref Unsafe.AsRef<T>( Unmanaged ); }
            else if (Boxed is not null) { return ref Boxed.GetReference(); }
            else { throw new NullReferenceException(); }
        }
    }
}

#if NET8_0_OR_GREATER
public readonly ref struct ManagedGoPointer<T> : IRefBox<T>
    where T : struct
{
    public readonly Box<T>? Boxed;
    public readonly ref T Managed;

    public ManagedGoPointer(Box<T>? boxed)
    {
        Boxed = boxed;
    }

    public ManagedGoPointer(ref T managed)
    {
        Managed = ref managed;
    }

    public bool IsNull => Boxed is null && Unsafe.IsNullRef(in Managed);

    public ref T Value
    {
        get
        {
            if (!Unsafe.IsNullRef(in Managed)) { return ref Managed; }
            else if (Boxed is not null) { return ref Boxed.GetReference(); }
            else { throw new NullReferenceException(); }
        }
    }
}
#endif