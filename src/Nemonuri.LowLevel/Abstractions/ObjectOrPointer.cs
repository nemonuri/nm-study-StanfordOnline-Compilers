
using System.Diagnostics;
using Nemonuri.LowLevel.Primitives;

namespace Nemonuri.LowLevel.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public readonly struct ObjectOrPointer : IEquatable<ObjectOrPointer>
{
    public readonly object? Object;
    public readonly nint Pointer;

    public bool IsNull => Equals(Null);

    [MemberNotNullWhen(true, [nameof(Object)])]
    public bool IsManaged => Object != default;

    public bool IsFixed => Pointer != default;

    public bool IsReference => Object is Reference;

    //--- Safe constructors ---
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ObjectOrPointer(object @object)
    {
        Object = @object;
        Pointer = default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ObjectOrPointer(void* pointer)
    {
        Object = default;
        Pointer = (nint)pointer;
    }

    public static readonly ObjectOrPointer Null = default;
    //---|

    //--- Primitive deconstructor ---
    public unsafe T DangerousCastOnce<T>()
    {
        if (IsFixed) { return RuntimePointerTheory.DangerousDereference<T>((void*)Pointer); }
        else if (IsManaged) { return (T)Object; }
        else { throw new NullReferenceException(); }
    }

    public T DangerousCast<T>()
    {
        Debug.Assert( !typeof(T).TypeHandle.Equals(typeof(Reference).TypeHandle) );

        if (!IsReference)
        {
            return DangerousCastOnce<T>();
        }

        return DangerousCastOnce<Reference>().Dereference().DangerousCast<T>();
    }
    //---|

    //--- compare equality ---
    public ObjectOrPointer GetSelfOrDereferenced()
    {
        if (!IsReference) { return this; }
        return DangerousCastOnce<Reference>().Dereference().GetSelfOrDereferenced();
    }

    public bool Equals(ObjectOrPointer other)
    {
        ObjectOrPointer ensuredThis = GetSelfOrDereferenced();
        ObjectOrPointer ensuredOther = GetSelfOrDereferenced();

        if (ensuredThis.IsNull || ensuredOther.IsNull) { return false; }

        return (ensuredThis.IsManaged, ensuredOther.IsManaged) switch
        {
            (true, true) => Equals(ensuredThis.Object, ensuredOther.Object),
            (false, false) => ensuredThis.Pointer == ensuredOther.Pointer,
            _ => false
        };
    }

    public override bool Equals(object? obj) => obj is ObjectOrPointer v && Equals(v);
    
    public override int GetHashCode()
    {
        var ensuredThis = GetSelfOrDereferenced();
        if (ensuredThis.IsManaged) { return ensuredThis.Object.GetHashCode(); }
        else if (ensuredThis.IsFixed) { return ensuredThis.Pointer.GetHashCode(); }
        else { return 0; }
    }
    //---|

    //--- Unsafe deconstructors ---
    // Note: These methods are not primitive...
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? DangerousAs<T>() where T : class
    {
        return Unsafe.As<T>(Object);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T DangerousUnbox<T>() where T : struct
    {
        return ref Unsafe.Unbox<T>(Object!);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ref T DangerousAsRef<T>() where T : unmanaged
    {
        return ref Unsafe.AsRef<T>((void*)Pointer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ref T DangerousRefCast<T>()
    {
        if (Pointer != default)
        {
            return ref Unsafe.As<byte, T>(ref Unsafe.AsRef<byte>((void*)Pointer));
        }
        else if (Object != null)
        {
            if (typeof(T).IsValueType)
            {
                ref byte rawData = ref Unsafe.As<Runtime.RawObjectData>(Object).Data;
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
    //---|

}
