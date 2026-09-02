using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SubarraysDistinctElementSumOfSquaresI;

// LeetCode 2913. Subarrays Distinct Element Sum of Squares I: sum, over every
// subarray, the square of its distinct-element count. n <= 100 per the problem's own
// constraints, so growing one Set<int> per start index as the end index sweeps
// rightward - counting distinct values without re-scanning from scratch - is already
// the intended O(n^2) solution, not just a baseline (unlike part II, LC 3410, which
// needs a Fenwick-tree-backed O(n log n) approach this repo has no facade for yet).
public sealed partial class SubarraysDistinctElementSumOfSquaresITests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [1, 2, 1], 15 },
            { [1, 1], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumOfSquares_LeetCodeExamples_ReturnsExpectedSum(int[] nums, long expected)
    {
        var sum = SumOfSquares(nums);

        Assert.Equal(expected, sum);
    }

    private static long SumOfSquares(int[] nums)
    {
        long total = 0;

        for (var start = 0; start < nums.Length; start++)
        {
            var distinct = new Set<int>();

            for (var end = start; end < nums.Length; end++)
            {
                distinct.TryAdd(nums[end]);
                total += (long)distinct.Count * distinct.Count;
            }
        }

        return total;
    }
}
