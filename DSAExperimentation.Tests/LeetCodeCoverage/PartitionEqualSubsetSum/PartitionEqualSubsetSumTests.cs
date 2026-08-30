using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PartitionEqualSubsetSum;

// LeetCode 416. Partition Equal Subset Sum: 0/1-knapsack subset-sum recurrence
// f(index, remaining) = remaining == 0 || (index < nums.Length && (f(index+1,
// remaining-nums[index]) || f(index+1, remaining))), via this repo's own
// Memoizer (CoinChange/WordBreak precedent) instead of the textbook
// un-memoized exponential recursion - each (index, remaining) state is solved
// once and reused across every branch that lands back on it.
public sealed partial class PartitionEqualSubsetSumTests
{
    [Theory]
    [InlineData(new[] { 1, 5, 11, 5 }, true)]
    [InlineData(new[] { 1, 2, 3, 5 }, false)]
    [InlineData(new[] { 1, 2, 5 }, false)]
    public void CanPartition_LeetCodeExamples_ReturnsWhetherEqualSplitExists(int[] nums, bool expected)
        => Assert.Equal(expected, CanPartition(nums));

    private static bool CanPartition(int[] nums)
    {
        var total = nums.Sum();
        if (total % 2 != 0)
        {
            return false;
        }

        var half = total / 2;
        return Memoizer.Memoize<(int Index, int Remaining), bool>((0, half), CanReach);

        bool CanReach((int Index, int Remaining) state, Func<(int Index, int Remaining), bool> canReach)
        {
            if (state.Remaining == 0)
            {
                return true;
            }

            if (state.Remaining < 0 || state.Index == nums.Length)
            {
                return false;
            }

            return canReach((state.Index + 1, state.Remaining - nums[state.Index]))
                || canReach((state.Index + 1, state.Remaining));
        }
    }
}
