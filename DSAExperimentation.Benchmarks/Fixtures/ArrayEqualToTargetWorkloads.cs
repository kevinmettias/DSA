namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3229 - target is kept within a small delta of nums
// rather than spanning the full 1-1e8 range LC allows, because the brute-force
// simulation arm pays per unit of every diff it walks down to zero: a full-range delta
// would make it take millions of steps per element and turn the "baseline" arm into a
// timeout rather than a meaningful comparison, the same reasoning TwoSumBenchmarks
// gives for tuning its own target.
internal static class ArrayEqualToTargetWorkloads
{
    private const int MinValue = 1;
    private const int ValueSpread = 200;
    private const int MaxDelta = 6;

    public static (int[] Nums, int[] Target) BuildArrays(int length, int seed)
    {
        var random = new Random(seed);
        var nums = new int[length];
        var target = new int[length];

        for (var i = 0; i < length; i++)
        {
            nums[i] = random.Next(MinValue, MinValue + ValueSpread);
            var delta = random.Next(-MaxDelta, MaxDelta + 1);
            target[i] = Math.Max(MinValue, nums[i] + delta);
        }

        return (nums, target);
    }
}
