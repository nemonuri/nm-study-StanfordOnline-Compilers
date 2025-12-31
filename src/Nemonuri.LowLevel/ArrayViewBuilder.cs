// Reference: https://github.com/dotnet/runtime/blob/main/src/coreclr/tools/Common/System/Collections/Generic/ArrayBuilder.cs

namespace Nemonuri.LowLevel;

public struct ArrayViewBuilder<T> : IMemoryView<T>, IMaybeSupportsRawSpan<T>
{
    private T[]? _array;
    private int _length;

    public ArrayViewBuilder(int initialCapacity = 4)
    {
        _array = new T[initialCapacity];
        _length = 0;
    }

    public T[] ToArray()
    {
        if (Length == 0) { return []; }

        T[] newArray = new T[Length];
        Array.Copy(AsArray, newArray, Length);
        return newArray;
    }

    public ArrayView<T> ToArrayView()
    {
        return new(ToArray());
    }

    public void Add(T item)
    {
        EnsureCapacity(Length+1);
        AsArray[_length] = item;
        _length += 1;
    }

    public void EnsureCapacity(int requestedCapacity)
    {
        if (!(requestedCapacity <= Capacity))
        {
            Grow(requestedCapacity);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int requestedCapacity)
    {
        int newCapacity = Math.Max(2 * Capacity + 1, requestedCapacity);
        Array.Resize(ref _array, newCapacity);
    }

    public T[] AsArray => _array ??= [];

    public readonly int Length => _length;
    public int Capacity => AsArray.Length;

    public ref T this[int index] => ref AsArray[index];

    public bool SupportsRawSpan => true;

    public Span<T> AsSpan => AsArray.AsSpan();
}
