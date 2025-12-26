
namespace Nemonuri.LowLevel;

public struct LowLevelKeyValuePair<TKey, TValue>
{
    public readonly TKey Key;
    public TValue Value;

    public LowLevelKeyValuePair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}
