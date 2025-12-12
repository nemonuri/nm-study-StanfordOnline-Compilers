using System.Runtime.CompilerServices;

namespace DscTool;

public static class UnsafeExtensions
{
    extension(Unsafe)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly TTo ReadOnlyAs<TFrom, TTo>(ref readonly TFrom from) =>
            ref Unsafe.As<TFrom, TTo>(ref Unsafe.AsRef(in from));
    }
}
