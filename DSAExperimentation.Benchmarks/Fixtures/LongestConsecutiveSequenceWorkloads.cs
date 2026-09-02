namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 128 - values are drawn from a range half
// the array's length, which guarantees heavy duplication and fragments the
// value space into many runs rather than one, so the "skip if predecessor
// present" check actually gets exercised instead of the array being one
// long run end to end.
internal static class LongestConsecutiveSequenceWorkloads
{
    public static int[] BuildArray(int length, int seed)
    {
        var random = new Random(seed);
        var nums = new int[length];
        var spread = Math.Max(1, length / 2);

        for (var i = 0; i < length; i++)
        {
            nums[i] = random.Next(0, spread);
        }

        return nums;
    }
}
