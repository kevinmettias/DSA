namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 164 - MaximumGapSolution's own methods copy
// and sort nums internally, so all this builds is how many random values to
// hand them.
internal static class MaximumGapWorkloads
{
    private const int ValueExclusiveBound = 1_000_000;

    public static int[] BuildValues(int length, int seed)
    {
        var random = new Random(seed);

        return [.. Enumerable.Range(0, length).Select(_ => random.Next(0, ValueExclusiveBound))];
    }
}
