
using System.Diagnostics;
using Nemonuri.LowLevel.Primitives;

namespace Nemonuri.LowLevel.DuckTyping;

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

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct RelationHandle<TLeft, TRight>
{
    public readonly delegate*<in TLeft?, in TRight?, bool> FunctionPointer;

    public RelationHandle(delegate*<in TLeft?, in TRight?, bool> functionPointer)
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
    public bool InvokeRelation(in TLeft? left, in TRight? right) => FunctionPointer(in left, in right);
}

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct ProviderHandle<TProvider, TItem>
{
    public readonly delegate*<in TProvider?, ref readonly TItem?> FunctionPointer;

    public ProviderHandle(delegate*<in TProvider?, ref readonly TItem?> functionPointer)
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
    public ref readonly TItem? InvokeProvider(in TProvider? source) => ref FunctionPointer(in source);
}
