namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3636 - a deterministic array over a small value
// alphabet (so real majorities exist to find, rather than every query bottoming out
// at threshold 1) plus a batch of random-but-valid [l, r, threshold] queries.
internal static class ThresholdMajorityQueriesWorkloads
{
    private const int ValueAlphabetSize = 20;
    private const int QueryCount = 1_000;

    public static int[] BuildNums(int count, int seed)
    {
        var random = new Random(seed);
        var nums = new int[count];

        for (var i = 0; i < count; i++)
        {
            nums[i] = random.Next(1, ValueAlphabetSize + 1);
        }

        return nums;
    }

    public static int[][] BuildQueries(int elementCount, int seed)
    {
        var random = new Random(seed);
        var queries = new int[QueryCount][];

        for (var q = 0; q < QueryCount; q++)
        {
            var l = random.Next(elementCount);
            var r = l + random.Next(elementCount - l);
            var threshold = random.Next(1, (r - l + 1) + 1);
            queries[q] = [l, r, threshold];
        }

        return queries;
    }
}
