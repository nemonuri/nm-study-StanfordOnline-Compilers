
namespace DscTool.Infrastructure;

public interface IReadOnlyMap<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    where TKey : IEquatable<TKey>
{
    public TValue Fallback {get;}
}
