namespace DSAExperimentation.Collections;

// Shared tuning knobs for every array-backed Collections/* structure's growth
// strategy (DynamicArray, CircularBuffer, HashMap's entries/buckets) - not a shared
// Representation type, just the numbers each independently doubles by.
internal static class ArrayGrowth
{
    public const int InitialCapacity = 4;
    public const int GrowthFactor = 2;
}
