
using System.Runtime.InteropServices;

namespace DscTool.Infrastructure;

[StructLayout(LayoutKind.Sequential)]
public readonly struct PackedMap<TKey, TValue> where TKey : IEquatable<TKey>
{
    public Memory<RawKeyValuePair<TKey, TValue>> Memory {get;}
    public TValue Fallback {get;}

    public PackedMap(Memory<RawKeyValuePair<TKey, TValue>> memory, TValue fallback)
    {
        Memory = memory;
        Fallback = fallback;
    }

    public Span<RawKeyValuePair<TKey, TValue>> AsSpan => Memory.Span;

    public RawKeyValuePair<OptionalKey<TKey>, TValue> GetValueOrFallback(TKey key)
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
}
