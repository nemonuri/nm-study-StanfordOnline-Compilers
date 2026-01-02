
using System.Diagnostics;
using Nemonuri.LowLevel.Primitives;

namespace Nemonuri.LowLevel;

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct MethodHandle<TReceiver, TArgument, TResult>
{
    public readonly delegate*<ref TReceiver, ref TArgument?, ref TResult?> FunctionPointer;

    public MethodHandle(delegate*<ref TReceiver, ref TArgument?, ref TResult?> functionPointer)
    {
        Debug.Assert( !RuntimePointerTheory.IsUndefinedOrNullPointer(functionPointer) );
        FunctionPointer = functionPointer;
    }
}

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct FunctionHandle<TArgument, TResult>
{
    public readonly delegate*<ref TArgument?, ref TResult?> FunctionPointer;

    public FunctionHandle(delegate*<ref TArgument?, ref TResult?> functionPointer)
    {
        Debug.Assert( !RuntimePointerTheory.IsUndefinedOrNullPointer(functionPointer) );
        FunctionPointer = functionPointer;
    }
}
