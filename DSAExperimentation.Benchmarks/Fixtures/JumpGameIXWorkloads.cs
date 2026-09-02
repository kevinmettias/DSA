namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3660 - a random permutation-like value array, so
// both the number of components and the split points between them vary rather than
// collapsing to one trivial case.
internal static class JumpGameIXWorkloads
{
    private const int MaxValue = 1_000_000_000;

    public static int[] Build(int count, int seed)
    {
        var random = new Random(seed);
        var nums = new int[count];

        for (var i = 0; i < count; i++)
        {
            nums[i] = random.Next(1, MaxValue + 1);
        }

        return nums;
    }
}
