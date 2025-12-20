
using System.Runtime.InteropServices;

namespace DscTool.Infrastructure;

[StructLayout(LayoutKind.Sequential)]
public readonly struct RawKeyValuePair<TKey, TValue>
    where TKey : IEquatable<TKey>
{
    public readonly TKey Key;
    public readonly TValue Value;

    public RawKeyValuePair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}

