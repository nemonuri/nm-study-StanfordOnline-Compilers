namespace Nemonuri.LowLevel.Primitives.DotNet;

partial struct TypeInfo
{
    private static class Flags
    {
        public const uint WellKnownPropertiesAssigned = 1 << 0;

        public const uint EnumPropertiesAssigned = 1 << 1;

        public const uint NullablePropertiesAssigned = 1 << 2;

        public const uint ClassLayoutPropertiesAssigned = 1 << 3;

        public const uint InstanceFieldsPropertiesAssigned = 1 << 4;
    }
}
