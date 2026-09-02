namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3901 - nums and update values drawn from a
// range wide enough that most positions are not multiples of P, so most queries
// settle in the composed strategy's O(log n) range-query path rather than its
// O(n) fallback (GoodSubsequenceQueriesSolution.HasGoodSubsequenceByRangeQuery),
// the same "the smart strategy wins the common case" story SegmentTree already
// tells for LC 3605's window gcds.
internal static class GoodSubsequenceQueriesWorkloads
{
    public const int P = 3;

    private const int MaxValue = 50;

    public static (int[] Nums, int[][] Queries) Build(int length, int queryCount, int seed)
    {
        var random = new Random(seed);
        var nums = new int[length];

        for (var i = 0; i < length; i++)
        {
            nums[i] = random.Next(1, MaxValue + 1);
        }

        var queries = new int[queryCount][];

        for (var i = 0; i < queryCount; i++)
        {
            queries[i] = [random.Next(length), random.Next(1, MaxValue + 1)];
        }

        return (nums, queries);
    }
}
