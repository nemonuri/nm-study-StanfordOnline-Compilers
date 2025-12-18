
namespace DscTool.Infrastructure;

public interface IMemoryLike<T>
{
    Span<T> Span {get;}
}

public interface IReadOnlyMemoryLike<T>
{
    ReadOnlySpan<T> Span {get;}
}

public interface IReadOnlyMemoryLikeDictionary<TKey, TValue> : 
    IReadOnlyDictionary<TKey, TValue>, 
    IReadOnlyMemoryLike<KeyValuePair<TKey, TValue>>
{
}

public interface IMemoryLikeDictionary<TKey, TValue> : 
    IDictionary<TKey, TValue>, 
    IMemoryLike<KeyValuePair<TKey, TValue>>
{
}
