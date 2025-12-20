
using System.Diagnostics;

namespace DscTool.Infrastructure;

public readonly struct ReadOnlyDictionaryFallbackPair<TKey, TValue>
{
    public readonly IReadOnlyDictionary<TKey, TValue> Dictionary;
    public readonly TValue Fallback;

    public ReadOnlyDictionaryFallbackPair(IReadOnlyDictionary<TKey, TValue> dictionary, TValue fallback)
    {
        Debug.Assert(dictionary is not null);
        Debug.Assert(fallback is not null);

        Dictionary = dictionary!;
        Fallback = fallback;
    }
}
