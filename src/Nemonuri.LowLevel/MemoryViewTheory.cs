
namespace Nemonuri.LowLevel;

public static class MemoryViewTheory
{
    extension
    <TView, TMemoryView>
    (scoped in 
        TheoryBox<TView, TMemoryView>
        theory)
        where TMemoryView : IMemoryView<TView>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        public static TheoryBox<TView, TMemoryView>
        Theorize
        (in TMemoryView source)
        =>
        TheoryBox.Box
        <TView, TMemoryView>
        (in source);

        public bool TryGet<TContext>
        (
            in TContext? context,
            PredicateHandle<TContext, TView> predicateHandle,
            out TView? resultItem,
            out int resultIndex
        )
        {
            ref readonly TMemoryView self = ref theory.Self;
            for (int i = 0; i < self.Length; i++)
            {
                ref var item = ref self[i];
                if (predicateHandle.Invoke(in context, in self[i]))
                {
                    resultItem = item;
                    resultIndex = i;
                    return true;
                }
            }

            resultItem = default;
            resultIndex = -1;
            return false;
        }
    }

    extension
    <TView, TMemoryView>
    (scoped in 
        TheoryBox<TView, TMemoryView>
        theory)
        where TMemoryView : IMemoryView<TView>
    {
        public unsafe MemoryViewReceiver<TMemoryView, TView> ToLowLevelAbstrct()
        {
            static int LengthGetter(in TMemoryView handler) => handler.Length;
            static ref TView ItemGetter(ref TMemoryView handler, int index) => ref handler[index];

            return new(theory.Self, new(&LengthGetter, &ItemGetter));
        }
    }

    extension
    <TKey, TValue, TMemoryView>
    (scoped in TheoryBox<LowLevelKeyValuePair<TKey, TValue>, TMemoryView> theory)
        where TMemoryView : IMemoryView<LowLevelKeyValuePair<TKey, TValue>>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif    
        where TKey : IEquatable<TKey>
    {
        public unsafe bool TryGetEntry
        (
            in TKey key,
            out LowLevelKeyValuePair<TKey, TValue> resultEntry,
            out int resultIndex
        )
        {
            static bool AreKeyEqual
            (
                in TKey? leftKey,
                in LowLevelKeyValuePair<TKey, TValue> rightPair
            )
            {
                return Internal.StaticMethods.AreEquivalent(in leftKey, in rightPair.Key);
            }

            return theory.TryGet(in key, new(&AreKeyEqual), out resultEntry, out resultIndex);
        }
    }
    
}