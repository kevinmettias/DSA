namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3653 - random starting values plus a batch of
// random range-multiplication queries, each with its own random stride and
// multiplier, so both strategies see the same mix of wide and narrow strides a real
// query set would contain.
internal static class XORAfterRangeMultiplicationQueriesIWorkloads
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

        var queries = new int[queryCount][];

        for (var i = 0; i < queryCount; i++)
        {
            var l = random.Next(nodeCount);
            var r = random.Next(l, nodeCount);
            var k = random.Next(1, nodeCount + 1);
            var v = random.Next(1, MaxMultiplier + 1);

            queries[i] = [l, r, k, v];
        }

        return (nums, queries);
    }
}
