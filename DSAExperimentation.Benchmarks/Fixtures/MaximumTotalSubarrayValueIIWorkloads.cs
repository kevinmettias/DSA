namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3691 - values are drawn from the problem's full
// range so max/min rarely coincide across a window, keeping every candidate's
// value genuinely data-dependent rather than trivially zero.
internal static class MaximumTotalSubarrayValueIIWorkloads
{
    private const int ValueUpperBound = 1_000_000_000;

    public static int[] BuildNums(int size, int seed)
    {
        var random = new Random(seed);
        var nums = new int[size];

        for (var i = 0; i < size; i++)
        {
            nums[i] = random.Next(ValueUpperBound);
        }

        return nums;
    }
}
