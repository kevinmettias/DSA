namespace DSAExperimentation.Benchmarks.Fixtures;

// The window bound the LC 3578 scenario MaxMinPartitionWorkloads is sized for. Separate
// from MaxMinPartitionWorkloads because it is the scenario's value, which a benchmark has
// to name to drive the same case, not part of how the array is generated.
internal static class MaxMinPartitionScenario
{
    // Comfortably above the value range, so every window is valid and every position's
    // segment stretches back to the start of the array.
    public const int MaxMinDifference = 1_000_000;
}
