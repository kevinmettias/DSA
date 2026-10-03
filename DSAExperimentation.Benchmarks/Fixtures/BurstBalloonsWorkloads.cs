namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 312 - a random scattering of balloon values, at
// whatever count an arm asks for. The benchmark keeps its exponential baseline to 14
// balloons and runs the memoized arm on to LC 312's bound of 300.
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
