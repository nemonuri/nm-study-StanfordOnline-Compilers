
namespace DscTool.Infrastructure.Extensions;

public static class PackedMap
{
    extension<TKey, TValue>
    (scoped ref readonly PackedMap<TKey, TValue> self)
        where TKey : IEquatable<TKey>
        where TValue : IEquatable<TValue>
    {
        public bool ExtensionallyEquals(scoped ref readonly PackedMap<TKey, TValue> other)
        {
            if (!UnsafeReadOnly.AreNotNullRef(in self, in other)) {return false;}
            if (UnsafeReadOnly.AreSameRef(in self, in other)) {return true;}

            int length = self.Length;
            if (length != other.Length) {return false;}
            if (!self.Fallback.Equals(other.Fallback)) {return false;}

            foreach (var selfEntry in self.AsSpan)
            {
                var selfKey = selfEntry.Key;

                RawKeyValuePair<OptionalKey<TKey>, TValue> otherEntry = other.GetEntryOrFallback(selfKey);
                if (!otherEntry.Key.IsSome) {return false;}
                if (!otherEntry.Key.GetSome().Equals(selfKey)) {return false;}
                if (!selfEntry.Value.Equals(otherEntry.Value)) {return false;}
            }

            return true;
        }

        public int GetExtensionalHashCode()
        {
            if (UnsafeReadOnly.IsNullRef(in self)) {return 0;}

            int hashCode = self.Fallback.GetHashCode();
            foreach (var entry in self.AsSpan)
            {
                hashCode ^= entry.Key.GetHashCode();
                hashCode ^= entry.Value.GetHashCode();
            }
            return hashCode;
        }
    }
}
