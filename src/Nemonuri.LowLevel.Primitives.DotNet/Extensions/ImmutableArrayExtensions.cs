
namespace Nemonuri.LowLevel.Primitives.DotNet.Extensions;

public static class ImmutableArrayExtensions
{

    extension<T>(in ImmutableArray<T> self)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[]? MarshalAsArray() => ImmutableCollectionsMarshal.AsArray(self);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArray<TOut> ConvertAll<TOut>(Converter<T, TOut> converter) => self.MarshalAsArray().ConvertAll(converter).MarshalAsImmutableArray();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ImmutableArray<TOut> ConvertAll<TOut>(delegate*<T, TOut> converter) => self.MarshalAsArray().ConvertAll(converter).MarshalAsImmutableArray();
    }
}
