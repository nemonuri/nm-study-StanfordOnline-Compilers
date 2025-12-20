using DscTool.Infrastructure;

namespace DscTool.Scoped.Hashtables;

public readonly partial struct ScopedDictionaryHoareTripleCommand<T, TCondition, TCommand, TKey> : 
    IScopedHoareTripleCommand<
        Memory<KeyValuePair<TKey, T>>, 
        ReadOnlyDictionaryFallbackPair<TKey, TCondition>>
    where TKey : IEquatable<TKey>
    where TCommand : IScopedHoareTripleCommand<T, TCondition>
{
    private readonly ReadOnlyDictionaryFallbackPair<TKey, TCommand> _command;
    private readonly ReadOnlyDictionaryFallbackPair<TKey, TCondition> _precondition;
    private readonly TCondition _postConditionFallback;

    public ScopedDictionaryHoareTripleCommand
    (
        ReadOnlyDictionaryFallbackPair<TKey, TCommand> command,
        TCondition postConditionFallback
    )
    {
        _command = command;
        _precondition = new(new PreConditionTable(_command.Dictionary), _command.Fallback.PreCondition);
        _postConditionFallback = postConditionFallback;
    }

    [UnscopedRef] public ref readonly ReadOnlyDictionaryFallbackPair<TKey, TCondition> PreCondition => ref _precondition;

    public bool TryInvoke
    (
        scoped ref readonly Memory<KeyValuePair<TKey, T>> source, 
        [NotNullWhen(true)] scoped ref Memory<KeyValuePair<TKey, T>> result, 
        [NotNullWhen(true)] scoped ref ReadOnlyDictionaryFallbackPair<TKey, TCondition> postCondition
    )
    {
        var sourceSpan = source.Span;
        var resultSpan = result.Span;
        int length = sourceSpan.Length;
        if (length != resultSpan.Length) {return false;}
        var postConditionArray = new KeyValuePair<TKey, TCondition>[length];

        T? resultValue = default;
        TCondition? postConditionValue = default;
        for (int i = 0; i < length; i++)
        {
            ref readonly var s = ref sourceSpan[i];
            var sourceValue = s.Value;

            if (!_command.GetValueOrFallback(s.Key).TryInvoke(in sourceValue, ref resultValue, ref postConditionValue)) {return false;}
            resultSpan[i] = new(key: s.Key, value: resultValue);
            postConditionArray[i] = new(key: s.Key, value: postConditionValue);
        }
        postCondition = new(dictionary: new PostConditionTable(postConditionArray), fallback: _postConditionFallback);
        return true;
    }
}
