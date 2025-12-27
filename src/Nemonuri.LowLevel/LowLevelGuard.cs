namespace Nemonuri.LowLevel;

public static class LowLevelGuard
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void IsNotNull(void* value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        if (value == null)
        {
            // Reference: https://github.com/CommunityToolkit/dotnet/blob/main/src/CommunityToolkit.Diagnostics/Internals/Guard.ThrowHelper.cs

            throw new ArgumentNullException(name, $"Parameter \"{name}\" ({typeof(void*).ToTypeString()}) must be not null.");
        }
    }
}
