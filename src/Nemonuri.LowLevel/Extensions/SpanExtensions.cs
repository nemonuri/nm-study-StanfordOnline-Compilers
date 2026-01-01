
namespace Nemonuri.LowLevel.Extensions;

public static class SpanExtensions
{
    extension<T>(Span<T> self)
    {
        public SpanView<T, TView> ToView<TView>(RefSelectorHandle<T, TView> selectorHandle)
        {
            return new(self, selectorHandle);
        }

        public unsafe SpanView<T, T> ToIdentityView()
        {
            return new(self, new(&Internal.StaticMethods.Identity));
        }
    }

    extension<TKey, TValue>(Span<RawKeyValuePair<TKey, TValue>> self)
        where TKey : IEquatable<TKey>
    {
        public unsafe bool TryGetEntry<TContext>
        (
            in TContext? context, 
            delegate*<in TContext?, in TKey, bool> predicate,
            out RawKeyValuePair<TKey, TValue> resultEntry,
            out int resultIndex
        )
        {
            Guard.IsTrue(predicate is not null);

            for (int i = 0; i < self.Length; i++)
            {
                ref var entry = ref self[i];
                if (predicate(in context, in entry.Key))
                {
                    resultEntry = entry;
                    resultIndex = i;
                    return true;
                }
            }

            resultEntry = default;
            resultIndex = -1;
            return false;
        }

        public unsafe bool TryGetEntry
        (
            in TKey key,
            out RawKeyValuePair<TKey, TValue> resultEntry,
            out int resultIndex
        )
        {
            static bool AreKeyEqual(in TKey leftKey, in TKey rightKey)
            {
                if (!UnsafeReadOnly.AreNotNullRef(in leftKey, in rightKey)) {return false;}
                if (UnsafeReadOnly.AreSameRef(in leftKey, in rightKey)) {return true;}

                return leftKey?.Equals(rightKey) ?? false;
            }

#pragma warning disable CS8622
            return self.TryGetEntry(in key, &AreKeyEqual, out resultEntry, out resultIndex);
#pragma warning restore CS8622
        }
    }
}
