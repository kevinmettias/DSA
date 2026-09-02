namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3420 - values are random, not sorted, within
// this file's own tighter range rather than the problem's full 1 <= nums[i] <= 1e9
// bound: values that noisy would blow any reasonable k within the first couple of
// elements of almost every subarray, so both strategies would barely do more than
// one comparison per left endpoint. A tighter range keeps a genuine mix of
// subarrays landing on both sides of the budget, so the sliding window actually
// grows and shrinks instead of resetting immediately.
internal static class NonDecreasingSubarrayWorkloads
{
    private const int ValueUpperBound = 1_000;

    public static int[] BuildNums(int size, int seed)
    {
        var random = new Random(seed);
        var nums = new int[size];

        for (var i = 0; i < size; i++)
        {
            nums[i] = random.Next(1, ValueUpperBound);
        }

        return nums;
    }
}
