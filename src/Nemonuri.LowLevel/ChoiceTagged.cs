
namespace Nemonuri.LowLevel;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Choice1Tagged<T>(T value)
{
    public readonly T Value = value;
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct Choice2Tagged<T>(T value)
{
    public readonly T Value = value;
}

