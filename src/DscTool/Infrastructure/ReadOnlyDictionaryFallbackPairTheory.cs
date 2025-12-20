
namespace DscTool.Infrastructure;

public static class ReadOnlyDictionaryFallbackPairTheory
{
    extension<TKey, TValue>(ReadOnlyDictionaryFallbackPair<TKey, TValue> self)
    {
        public TValue GetValueOrFallback(TKey key)
        {
            if (!self.Dictionary.TryGetValue(key, out var v)) {return v;}
            return self.Fallback;
        }
    }
}
