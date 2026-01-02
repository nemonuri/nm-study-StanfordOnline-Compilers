
using System.Diagnostics;
using Nemonuri.LowLevel.Primitives;

namespace Nemonuri.LowLevel;

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct MethodHandle<TReceiver, TSource, TResult>
{
    public readonly delegate*<ref TReceiver, ref TSource?, ref TResult?> FunctionPointer;

    public MethodHandle(delegate*<ref TReceiver, ref TSource?, ref TResult?> functionPointer)
    {
        Debug.Assert( !RuntimePointerTheory.IsUndefinedOrNullPointer(functionPointer) );
        FunctionPointer = functionPointer;
    }

    public bool IsNull 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => FunctionPointer == null; 
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TResult? InvokeMethod(ref TReceiver receiver, ref TSource? source) =>
        ref FunctionPointer(ref receiver, ref source);
}

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct FunctionHandle<TSource, TResult>
{
    public readonly delegate*<ref TSource?, ref TResult?> FunctionPointer;

    public FunctionHandle(delegate*<ref TSource?, ref TResult?> functionPointer)
    {
        Debug.Assert( !RuntimePointerTheory.IsUndefinedOrNullPointer(functionPointer) );
        FunctionPointer = functionPointer;
    }

    public bool IsNull 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => FunctionPointer == null; 
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref TResult? InvokeFunction(ref TSource? source) => ref FunctionPointer(ref source);
}
