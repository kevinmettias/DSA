namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3500 - nums and cost drawn independently from
// LC's own [1, 1000] value range.
internal static class MinimumCostToDivideArrayIntoSubarraysWorkloads
{
    private const int MaxValueInclusive = 1000;

    public static (int[] Nums, int[] Cost) Build(int length, int seed)
    {
        var random = new Random(seed);
        var nums = new int[length];
        var cost = new int[length];

        for (var i = 0; i < length; i++)
        {
            nums[i] = random.Next(1, MaxValueInclusive + 1);
            cost[i] = random.Next(1, MaxValueInclusive + 1);
        }

        return (nums, cost);
    }
}
