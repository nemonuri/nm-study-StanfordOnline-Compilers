namespace Nemonuri.LowLevel.Primitives.DotNet;

internal partial struct FieldInfo
{
    private class Flags
    {
        public const int WellKnownPropertiesAssigned = 1 << 0;

        public const int OrdinalPropertiesAssigned = 1 << 1;

        public const int OffsetPropertiesAssigned = 1 << 2;
    }

}
