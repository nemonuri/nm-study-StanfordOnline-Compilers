
using System.Runtime.InteropServices;

namespace DscTool.Infrastructure;

[StructLayout(LayoutKind.Sequential)]
public readonly struct PackedMap<TKey, TValue> where TKey : IEquatable<TKey>
{
    public readonly Memory<RawKeyValuePair<TKey, TValue>> Memory;
    public readonly TValue Fallback;

    public PackedMap(Memory<RawKeyValuePair<TKey, TValue>> memory, TValue fallback)
    {
        Memory = memory;
        Fallback = fallback;
    }

    public Span<RawKeyValuePair<TKey, TValue>> AsSpan => Memory.Span;

    public int Length => Memory.Length;

    public bool ContainsKey(TKey key)
    {
        foreach (ref readonly RawKeyValuePair<TKey, TValue> entry in AsSpan)
        {
            if (entry.Key.Equals(key))
            {
                return true;
            }
        }
        return false;
    }

    public RawKeyValuePair<OptionalKey<TKey>, TValue> GetEntryOrFallback(TKey key)
    {
        foreach (ref readonly RawKeyValuePair<TKey, TValue> entry in AsSpan)
        {
            if (entry.Key.Equals(key))
            {
                return new (key: OptionalKeyTagger.Some(key), value: entry.Value);
            }
        }
        return new (key: OptionalKeyTagger.None, value: Fallback);
    }

    public RawKeyValuePair<OptionalKey<TKey>, TValue> GetEntryOrFallback(OptionalKey<TKey> optionalKey)
    {
        if (!optionalKey.IsSome) {return new (key: OptionalKeyTagger.None, value: Fallback);}
        return GetEntryOrFallback(optionalKey.GetSome());
    }

    public ref TValue GetValueRef(TKey key)
    {
        var span = AsSpan;
        for (int i = 0; i < span.Length; i++)
        {
            if (span[i].Key.Equals(key)) {return ref span[i].Value;}
        }
        throw new ArgumentOutOfRangeException($"Cannot find key. key = {key}");
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
