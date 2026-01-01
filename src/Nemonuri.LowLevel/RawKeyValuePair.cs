
namespace Nemonuri.LowLevel;

public struct RawKeyValuePair<TKey, TValue>
{
    public readonly TKey Key;
    public TValue Value;

    public RawKeyValuePair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}
