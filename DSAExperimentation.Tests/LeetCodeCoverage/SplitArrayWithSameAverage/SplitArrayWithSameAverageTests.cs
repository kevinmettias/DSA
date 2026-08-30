using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SplitArrayWithSameAverage;

// LeetCode 805. Split Array With Same Average: avg(A) == avg(B) reduces to "does some
// proper nonempty subset of size k (1 <= k <= n/2 by symmetry with its complement) sum
// to total*k/n" - a 0/1-knapsack-with-a-required-count recurrence over (index,
// countNeeded, sumNeeded), via this repo's own Memoizer, the same
// PartitionEqualSubsetSumTests.cs precedent with one extra dimension (a required
// subset SIZE, not just a required subset SUM).
public sealed partial class SplitArrayWithSameAverageTests
{
    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7, 8 }, true)]
    [InlineData(new[] { 3, 1 }, false)]
    public void CanSplitWithSameAverage_LeetCodeExamples_ReturnsWhetherSplitExists(int[] nums, bool expected)
        => Assert.Equal(expected, CanSplitWithSameAverage(nums));

    [Fact]
    public void CanSplitWithSameAverage_SingleElement_ReturnsFalse()
        => Assert.False(CanSplitWithSameAverage([5]));

    private static bool CanSplitWithSameAverage(int[] nums)
    {
        var n = nums.Length;
        var total = nums.Sum();

        for (var k = 1; k <= n / 2; k++)
        {
            if (total * k % n != 0)
            {
                continue;
            }

            var targetSum = total * k / n;
            if (Memoizer.Memoize<(int Index, int Count, int Sum), bool>((0, k, targetSum), CanReach))
            {
                return true;
            }
        }

        return false;

        bool CanReach((int Index, int Count, int Sum) state, Func<(int Index, int Count, int Sum), bool> canReach)
        {
            if (state.Count == 0)
            {
                return state.Sum == 0;
            }

            if (state.Index == nums.Length || state.Sum < 0)
            {
                return false;
            }

            return canReach((state.Index + 1, state.Count, state.Sum))
                || canReach((state.Index + 1, state.Count - 1, state.Sum - nums[state.Index]));
        }
    }
}
