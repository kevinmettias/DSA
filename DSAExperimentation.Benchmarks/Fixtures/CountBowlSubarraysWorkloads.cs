namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3676 - LC guarantees distinct elements, so this
// shuffles a contiguous range rather than drawing independent random values, which
// would risk duplicates neither strategy is specified for.
internal static class CountBowlSubarraysWorkloads
{
    public static int[] BuildNums(int size, int seed)
    {
        var random = new Random(seed);
        var nums = new int[size];

        for (var i = 0; i < size; i++)
        {
            nums[i] = i + 1;
        }

        for (var i = size - 1; i > 0; i--)
        {
            var swapIndex = random.Next(i + 1);
            (nums[i], nums[swapIndex]) = (nums[swapIndex], nums[i]);
        }

        return nums;
    }
}
