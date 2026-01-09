
namespace Nemonuri.LowLevel.Primitives.DotNet.Extensions;

public static class ArrayExtensions
{
    extension<T>(T[]? self)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArray<T> MarshalAsImmutableArray() => ImmutableCollectionsMarshal.AsImmutableArray(self);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] GetEmptyIfNull() => self ?? [];

        public TOut[] ConvertAll<TOut>(Converter<T, TOut> converter) => Array.ConvertAll(self.GetEmptyIfNull(), converter);

        public unsafe TOut[] ConvertAll<TOut>(delegate*<T, TOut> converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            T[] array = self.GetEmptyIfNull();
            TOut[] newArray = new TOut[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = converter(array[i]);
            }
            return newArray;
        }
    }
}