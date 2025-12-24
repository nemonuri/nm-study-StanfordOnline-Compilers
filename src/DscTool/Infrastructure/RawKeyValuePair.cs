
using System.Runtime.InteropServices;

namespace DscTool.Infrastructure;

[StructLayout(LayoutKind.Sequential)]
public struct RawKeyValuePair<TKey, TValue>
    where TKey : IEquatable<TKey>
{
    public readonly TKey Key;
    public TValue Value;

    public RawKeyValuePair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}

