
namespace Nemonuri.LowLevel.Abstractions;

public readonly record struct ObjectOrPointer
{
    public readonly object? Object;
    public readonly nint Pointer;

    public bool IsNull => Object == default && Pointer == default;

    public ObjectOrPointer(object @object)
    {
        Object = @object;
        Pointer = default;
    }

    public ObjectOrPointer(nint pointer)
    {
        Object = default;
        Pointer = pointer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObjectOrPointer Box<T>(T value) where T : struct
    {
        object box = value;
        return new(box);
    }

    public T? DangerousAs<T>() where T : class
    {
        return Unsafe.As<T>(Object);
    }

    public ref T DangerousUnbox<T>() where T : struct
    {
        return ref Unsafe.Unbox<T>(Object!);
    }

    public unsafe ref T DangerousAsRef<T>() where T : unmanaged
    {
        return ref Unsafe.AsRef<T>((void*)Pointer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ref T DangerousCast<T>()
    {
        if (Pointer != default)
        {
            return ref Unsafe.As<byte, T>(ref Unsafe.AsRef<byte>((void*)Pointer));
        }
        else if (Object != null)
        {
            if (typeof(T).IsValueType)
            {
                ref byte rawData = ref Unsafe.As<Internal.RawObjectData>(Object).Data;
                return ref Unsafe.As<byte, T>(ref rawData);
            }
            else
            {
                return ref Unsafe.As<object, T>(ref Unsafe.AsRef(in Object));
            }
        }
        else
        {
            throw new NullReferenceException();
        }
    }
}
