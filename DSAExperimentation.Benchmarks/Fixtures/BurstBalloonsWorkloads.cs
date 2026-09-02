namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 312 - a random scattering of balloon values. Kept
// modest (<=14 in the benchmark's own [Params]) because BurstBalloonsSolution's
// un-memoized baseline is genuinely exponential, the same reasoning
// FibonacciBenchmarks.cs's NaiveRecursive already documents.
internal static class BurstBalloonsWorkloads
{
    private const int MaxBalloonValueExclusive = 100;

    public static int[] BuildBalloons(int count, int seed)
    {
        var random = new Random(seed);
        var nums = new int[count];

        for (var i = 0; i < count; i++)
        {
            nums[i] = random.Next(1, MaxBalloonValueExclusive);
        }

        return nums;
    }
}
