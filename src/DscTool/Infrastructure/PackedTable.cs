
using System.Runtime.InteropServices;

namespace DscTool.Infrastructure;

[StructLayout(LayoutKind.Sequential)]
public readonly struct PackedMap<TKey, TValue> where TKey : IEquatable<TKey>
{
    public Memory<RawKeyValuePair<TKey, TValue>> Memory {get;}
    public readonly TValue Fallback;

    public PackedMap(Memory<RawKeyValuePair<TKey, TValue>> memory, TValue fallback)
    {
        Memory = memory;
        Fallback = fallback;
    }

    public Span<RawKeyValuePair<TKey, TValue>> AsSpan => Memory.Span;

    public int Length => Memory.Length;

    public RawKeyValuePair<OptionalKey<TKey>, TValue> GetEntryOrFallback(TKey key)
    {
        Span<RawKeyValuePair<TKey, TValue>>.Enumerator e = AsSpan.GetEnumerator();
        while (e.MoveNext())
        {
            if (e.Current.Key.Equals(key))
            {
                return new (key: OptionalKeyTagger.Some(key), value: e.Current.Value);
            }
        }
        return new (key: OptionalKeyTagger.None, value: Fallback);
    }

    public PackedMap<TKey, TResultValue> SelectValue<TResultValue>(Func<TValue, TResultValue> selector)
    {
        Guard.IsNotNull(selector);

        Span<RawKeyValuePair<TKey, TValue>> sourceSpan = AsSpan;
        int length = sourceSpan.Length;

        // Make result fallback
        TResultValue resultFallback = selector(Fallback);

        // Make result memory
        var resultMemoryArray = new RawKeyValuePair<TKey, TResultValue>[length];
        
        for (int i = 0; i < length; i++)
        {
            var ss = sourceSpan[i];
            resultMemoryArray[i] = new(key: ss.Key, value: selector(ss.Value));
        }

        // Make result PackedMap
        Memory<RawKeyValuePair<TKey, TResultValue>> resultMemory = new (resultMemoryArray);
        return new (memory: resultMemory, fallback: resultFallback);
    }
}
