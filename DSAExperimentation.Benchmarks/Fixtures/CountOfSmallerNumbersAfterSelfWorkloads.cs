namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 315 - CountOfSmallerNumbersAfterSelfSolution's
// own methods coordinate-compress and sweep nums internally, so all this builds is
// how many random values to hand them.
internal static class CountOfSmallerNumbersAfterSelfWorkloads
{
    private const int ValueBound = 10_000;

    public static int[] BuildNums(int length, int seed)
    {
        var random = new Random(seed);

        return [.. Enumerable.Range(0, length).Select(_ => random.Next(-ValueBound, ValueBound))];
    }
}
