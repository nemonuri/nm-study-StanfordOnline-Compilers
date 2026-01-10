namespace Nemonuri.LowLevel.Primitives.DotNet;

partial struct TypeInfo
{
    private static class Flags
    {
        public const int WellKnownPropertiesAssigned = 1 << 0;

        public const int EnumPropertiesAssigned = 1 << 1;

        public const int NullablePropertiesAssigned = 1 << 2;

        public const int ClassLayoutPropertiesAssigned = 1 << 3;

        public const int InstanceFieldsPropertiesAssigned = 1 << 4;

        public const int StableLayoutPropertiesAssigned = 1 << 5;

        public const int SizePropertiesAssigned = 1 << 6;
    }
}
