
using System.Diagnostics;
using Nemonuri.LowLevel.Primitives;

namespace Nemonuri.LowLevel.Abstractions;

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct MethodHandle
{
    public readonly delegate*<ObjectOrPointer, ObjectOrPointer, ObjectOrPointer> FunctionPointer;

    public MethodHandle(delegate*<ObjectOrPointer, ObjectOrPointer, ObjectOrPointer> functionPointer)
    {
        Debug.Assert( !RuntimePointerTheory.IsUndefinedOrNullPointer(functionPointer) );
        FunctionPointer = functionPointer;
    }
}

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct FunctionHandle
{
    public readonly delegate*<ObjectOrPointer, ObjectOrPointer> FunctionPointer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FunctionHandle(delegate*<ObjectOrPointer, ObjectOrPointer> functionPointer)
    {
        FunctionPointer = functionPointer;
    }

    public bool IsNull 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => FunctionPointer == null; 
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ObjectOrPointer InvokeFunction(ObjectOrPointer source) =>
        IsNull ? ObjectOrPointer.Null : FunctionPointer(source);
}


