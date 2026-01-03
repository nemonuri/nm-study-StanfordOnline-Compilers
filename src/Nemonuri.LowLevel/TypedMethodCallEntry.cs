
using Nemonuri.LowLevel.Abstractions;

namespace Nemonuri.LowLevel;

public readonly struct TypedMethodCallEntry
{
    public readonly RuntimeTypeHandle MethodHandleType;
    public readonly MethodCallEntry MethodCallEntry;

    public TypedMethodCallEntry(RuntimeTypeHandle methodHandleType, MethodCallEntry methodCallEntry)
    {
        MethodHandleType = methodHandleType;
        MethodCallEntry = methodCallEntry;
    }
}

