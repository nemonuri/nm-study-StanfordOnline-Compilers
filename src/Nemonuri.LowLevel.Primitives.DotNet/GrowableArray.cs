// Reference: https://github.com/dotnet/runtime/blob/main/src/coreclr/tools/Common/System/Collections/Generic/ArrayBuilder.cs

namespace Nemonuri.LowLevel.Primitives.DotNet;

public struct GrowableArray<T>
{
    private T[]? _array;
    private int _length;

    public GrowableArray(int initialCapacity)
    {
        _array = new T[initialCapacity];
        _length = 0;
    }

    public readonly T[] ToArray()
    {
        if (Length == 0) { return []; }

        T[] newArray = new T[Length];
        Array.Copy(_array!, newArray, Length);

        return newArray;
    }

    public void Add(in T item)
    {
        EnsureCapacity(Length+1);
        _array[_length] = item;
        _length += 1;
    }

    public int AddDefaultsAndGetLength(int count)
    {
        if (!(count >= 0)) {throw new ArgumentOutOfRangeException(nameof(count), count, "Should greater than or equal to 0."); }
        if (count == 0) {return _length;}

        EnsureCapacity(Length+count);
        _length += count;
        return _length;
    }

    [MemberNotNull([nameof(_array)])]
    private void EnsureCapacity(int requestedCapacity)
    {
        Debug.Assert( requestedCapacity > 0 );

        if (!(requestedCapacity <= Capacity))
        {
            Grow(requestedCapacity);
        }
        else
        {
            Debug.Assert(_array is not null);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    [MemberNotNull([nameof(_array)])]
    private void Grow(int requestedCapacity)
    {
        int newCapacity = Math.Max(2 * Capacity + 1, requestedCapacity);
        Array.Resize(ref _array, newCapacity);
    }

    public readonly int Length => _length;
    public readonly int Capacity => _array is { } a ? a.Length : 0;

    public ref T this[int index]
    {
        get
        {
            if (!((uint)index <= _length))
            {
                throw new IndexOutOfRangeException();
            }
            return ref _array![index];
        }
    } 

    public readonly Span<T> AsSpan => _array.AsSpan()[..Length];
}
