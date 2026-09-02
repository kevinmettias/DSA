namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3532 - nums is built as a sorted sequence
// with an occasional large jump mixed in among small ones, so the resulting
// components are neither one giant blob nor all singletons, and random queries
// land on a genuine mix of connected and disconnected pairs.
internal static class ProximityQueryWorkloads
{
    private const int MaxDiff = 5;
    private const int SmallGapExclusive = MaxDiff + 1; // guarantees some within-maxDiff gaps
    private const int LargeGapExtra = 50; // pushes well past maxDiff, breaking the chain

    public static (int[] Nums, int MaxDiff) BuildNums(int n, int seed)
    {
        var random = new Random(seed);
        var nums = new int[n];
        var value = 0;

        for (var i = 0; i < n; i++)
        {
            nums[i] = value;
            var gap = random.Next(4) == 0 ? MaxDiff + LargeGapExtra : random.Next(SmallGapExclusive);
            value += gap;
        }

        return (nums, MaxDiff);
    }

    public static int[][] BuildQueries(int n, int queryCount, int seed)
    {
        var random = new Random(seed);
        var queries = new int[queryCount][];

        for (var i = 0; i < queryCount; i++)
        {
            queries[i] = [random.Next(n), random.Next(n)];
        }

        return queries;
    }
}
