namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3655 - random starting values plus a batch of
// random range-multiplication queries. Half the queries get a small stride (at most
// sqrt(n)) so the sqrt-decomposition strategy's bucketed path is actually exercised,
// not just its direct-walk fallback; the other half get any stride, the same mix
// Part I's workload builds.
internal static class XORAfterRangeMultiplicationQueriesIIWorkloads
{
    private const int MaxStartingValue = 1_000_000_000;
    private const int MaxMultiplier = 100_000;

    public static (int[] Nums, int[][] Queries) Build(int nodeCount, int queryCount, int seed)
    {
        var random = new Random(seed);
        var nums = new int[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            nums[i] = random.Next(1, MaxStartingValue + 1);
        }

        var smallStrideCeiling = Math.Max(1, (int)Math.Sqrt(nodeCount));
        var queries = new int[queryCount][];

        for (var i = 0; i < queryCount; i++)
        {
            var k = i % 2 == 0
                ? random.Next(1, smallStrideCeiling + 1)
                : random.Next(1, nodeCount + 1);
            var l = random.Next(nodeCount);
            var r = random.Next(l, nodeCount);
            var v = random.Next(1, MaxMultiplier + 1);

            queries[i] = [l, r, k, v];
        }

        return (nums, queries);
    }
}
