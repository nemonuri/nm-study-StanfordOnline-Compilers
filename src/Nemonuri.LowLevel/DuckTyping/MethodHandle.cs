
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
#if NET9_0_OR_GREATER
    where TProvider : allows ref struct
    where TItem : allows ref struct
#endif
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

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct FactoryHandle<TSource, TResult>
#if NET9_0_OR_GREATER
    where TSource : allows ref struct
    where TResult : allows ref struct
#endif
{
    public readonly delegate*<in TSource?, TResult?> FunctionPointer;

    public FactoryHandle(delegate*<in TSource?, TResult?> functionPointer)
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
    public TResult? InvokeFactory(in TSource? source) => FunctionPointer(in source);
}

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct FactoryHandle<TSource1, TSource2, TResult>
#if NET9_0_OR_GREATER
    where TSource1 : allows ref struct
    where TSource2 : allows ref struct
    where TResult : allows ref struct
#endif
{
    public readonly delegate*<in TSource1?, in TSource2?, TResult?> FunctionPointer;

    public FactoryHandle(delegate*<in TSource1?, in TSource2?, TResult?> functionPointer)
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
    public TResult? InvokeFactory(in TSource1? source1, in TSource2? source2) => FunctionPointer(in source1, in source2);
}

[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct ActionHandle<TReceiver, TArgument>
#if NET9_0_OR_GREATER
    where TReceiver : allows ref struct
    where TArgument : allows ref struct
#endif
{
    public readonly delegate*<ref TReceiver, in TArgument?, void> FunctionPointer;

    public ActionHandle(delegate*<ref TReceiver, in TArgument?, void> functionPointer)
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
    public void InvokeAction(ref TReceiver receiver, in TArgument? argument) => FunctionPointer(ref receiver, in argument);
}
