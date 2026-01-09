
namespace Nemonuri.LowLevel.Primitives.DotNet.Extensions;

public static class RuntimeTypeHandleExtensions
{
    extension(in RuntimeTypeHandle thisHandle)
    {
        public bool IsNull 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => thisHandle.Value == default; 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type GetDotNetType() => Type.GetTypeFromHandle(thisHandle) ?? throw new ArgumentNullException(nameof(thisHandle));
    }
}

