namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3578. K comfortably exceeds the value range, so
// every window is valid and every position's segment stretches all the way back to
// the start of the array - BruteForce's backward rescan is worst-case O(n) per r
// (O(n^2) overall), the case SlidingWindowDeque's two-pointer sweep (left never
// has to advance) exists to collapse to O(n).
internal static class MaxMinPartitionWorkloads
{
    private const int ValueBound = 1_000;

    public const int MaxMinDifference = 1_000_000;

    public static int[] BuildNums(int length, int seed)
    {
        var random = new Random(seed);
        var nums = new int[length];

        for (var i = 0; i < length; i++)
        {
            nums[i] = random.Next(1, ValueBound);
        }

        return nums;
    }
}
