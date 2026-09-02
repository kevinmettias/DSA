namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 2971 - kept small (ArrayLength 16/20) so
// the O(2^n) brute-force subset arm stays tractable while still exercising
// the sorted-running-sum arm's O(n log n) path on the same input, the same
// small-n convention CountTheNumberOfGoodPartitionsBenchmarks uses for its
// own exponential baseline.
internal static class PolygonWorkloads
{
    public static int[] BuildSides(int count, int seed)
    {
        var random = new Random(seed);
        var sides = new int[count];

        for (var i = 0; i < count; i++)
        {
            sides[i] = random.Next(1, 1000);
        }

        return sides;
    }
}
