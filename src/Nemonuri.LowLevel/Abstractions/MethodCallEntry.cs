
namespace Nemonuri.LowLevel.Abstractions;

public readonly struct MethodCallEntry
{
    public readonly Reference Receiver;
    public readonly nint Argument;
    public readonly nint FunctionPointer;

    public MethodCallEntry(Reference receiver, nint argument, nint functionPointer)
    {
        Receiver = receiver;
        Argument = argument;
        FunctionPointer = functionPointer;
    }
}

