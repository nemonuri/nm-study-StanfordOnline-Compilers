
using DscTool.Infrastructure;

namespace DscTool.Scoped.Hashtables;

public readonly partial struct ScopedPackedMapHoareTripleCommand<T, TCondition, TCommand, TKey> : 
    IScopedHoareTripleCommand<
        PackedMap<TKey, T>, 
        PackedMap<TKey, TCondition>>
    where TKey : IEquatable<TKey>
    where TCommand : IScopedHoareTripleCommand<T, TCondition>
{
    private readonly PackedMap<TKey, TCommand> _commandMap;
    private readonly PackedMap<TKey, TCondition> _preconditionMap;

    public ScopedPackedMapHoareTripleCommand
    (
        PackedMap<TKey, TCommand> commandMap
    )
    {
        _commandMap = commandMap;
        _preconditionMap = _commandMap.SelectValue(static c => c.PreCondition);
    }

    [UnscopedRef] public ref readonly PackedMap<TKey, TCondition> PreCondition => ref _preconditionMap;

    public bool TryInvoke
    (
        scoped ref readonly PackedMap<TKey, T> sourceMap, 
        [NotNullWhen(true)] scoped ref PackedMap<TKey, T> resultMap, 
        [NotNullWhen(true)] scoped ref PackedMap<TKey, TCondition> postConditionMap
    )
    {
        //--- Make fallbacks ---
        T? resultFallback = default;
        TCondition? postConditionFallback = default;

        if (!_commandMap.Fallback.TryInvoke(in sourceMap.Fallback, ref resultFallback, ref postConditionFallback))
            {return false;}
        //---|

        //--- Make memory arraies ---
        var sourceSpan = sourceMap.AsSpan;
        int length = sourceSpan.Length;
        var resultMemoryArray = new RawKeyValuePair<TKey, T>[length];
        var postConditionMemoryArray = new RawKeyValuePair<TKey, TCondition>[length];

        for (int i=0; i < length; i++)
        {
            ref readonly RawKeyValuePair<TKey, T> sourceEntry = ref sourceSpan[i];

            T? resultValue = default;
            TCondition? resultCondition = default;
            if (!_commandMap.GetEntryOrFallback(sourceEntry.Key).Value.TryInvoke(in sourceEntry.Value, ref resultValue, ref resultCondition))
                {return false;}
            
            resultMemoryArray[i] = new(sourceEntry.Key, resultValue);
            postConditionMemoryArray[i] = new(sourceEntry.Key, resultCondition);
        }
        //---|

        resultMap = new(memory: new(resultMemoryArray), fallback: resultFallback);
        postConditionMap = new(memory: new(postConditionMemoryArray), fallback: postConditionFallback);
        return true;
    }
}
