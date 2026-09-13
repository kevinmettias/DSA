namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 805. Values are kept small (not LeetCode's
// full 0..10000 range) specifically to keep the DP's (index, count, sum) state
// space small enough to benchmark, the same "N kept modest" reasoning
// PredictTheWinnerBenchmarks/CanIWinBenchmarks already document for their own
// exponential-search comparisons.
internal static class SplitArrayWithSameAverageWorkloads
{
    private const int MaxValueExclusive = 30;

    public static int[] BuildValues(int length, int seed)
    {
        var random = new Random(seed);

        return [.. Enumerable.Range(0, length).Select(_ => random.Next(1, MaxValueExclusive))];
    }
}
