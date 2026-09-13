namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 912 - SortAnArraySolution's own methods sort a
// copy of nums internally, so all this builds is how many random values, spanning
// both signs, to hand them.
internal static class SortAnArrayWorkloads
{
    private const int ValueRange = 50_000;

    public static int[] BuildValues(int length, int seed)
    {
        var random = new Random(seed);

        return [.. Enumerable.Range(0, length).Select(_ => random.Next(-ValueRange, ValueRange))];
    }
}
