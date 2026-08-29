namespace DSAExperimentation.DataStructures.Cache;

// Shared by LruCache and LfuCache, the same "value is genuinely shared, not two
// coincidentally-identical copies" reasoning ArrayGrowth already established for
// DynamicArray/CircularBuffer/HashMap's own tuning constants: both caches enforce the
// identical capacity invariant for the identical reason.
internal static class CacheConstants
{
    public const string InvalidCapacityMessage = "Capacity must be at least 1.";
}
