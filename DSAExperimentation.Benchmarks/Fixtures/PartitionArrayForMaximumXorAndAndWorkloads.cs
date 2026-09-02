namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3630 - a deterministic array of random 30-bit
// values, the only input either strategy needs.
internal static class PartitionArrayForMaximumXorAndAndWorkloads
{
    private const int MaxValue = 1_000_000_000;

    public static int[] BuildNums(int count, int seed)
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
