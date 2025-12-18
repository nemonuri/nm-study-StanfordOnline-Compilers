
namespace DscTool.Infrastructure;

public readonly struct WrappedMemory<T> : IMemoryLike<T>
{
    private readonly Memory<T> _memory;

    public WrappedMemory(Memory<T> memory)
    {
        _memory = memory;
    }

    public Span<T> Span => _memory.Span;

    public Memory<T> Unwrap() => _memory;
}

public readonly struct WrappedReadOnlyMemory<T> : IReadOnlyMemoryLike<T>
{
    private readonly ReadOnlyMemory<T> _readOnlyMemory;

    public WrappedReadOnlyMemory(ReadOnlyMemory<T> readOnlyMemory)
    {
        _readOnlyMemory = readOnlyMemory;
    }

    public ReadOnlySpan<T> Span => _readOnlyMemory.Span;

    public ReadOnlyMemory<T> Unwrap() => _readOnlyMemory;
}


