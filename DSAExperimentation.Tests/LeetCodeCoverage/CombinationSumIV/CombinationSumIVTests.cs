using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CombinationSumIV;

// LeetCode 377. Combination Sum IV: counting recurrence
// f(remaining) = sum over nums <= remaining of f(remaining - num), via this repo's own
// Memoizer (CoinChange precedent) instead of the textbook un-memoized exponential
// recursion - each remaining target is solved once and reused across every number
// choice that lands back on it. Despite the problem's own "combination" wording, order
// matters (LeetCode counts [1,3] and [3,1] as two different combinations for target 4)
// - the recurrence already reflects that by trying every num at every remaining
// target, not just nums at-or-after the previous choice the way a true (order-
// independent) combination count would restrict the inner loop.
public sealed partial class CombinationSumIVTests
{
    [Theory]
    [InlineData(new[] { 1, 2, 3 }, 4, 7)]
    [InlineData(new[] { 9 }, 3, 0)]
    [InlineData(new[] { 1 }, 0, 1)]
    public void CountCombinations_LeetCodeExamples_ReturnsOrderSensitiveCount(int[] nums, int target, int expected)
        => Assert.Equal(expected, CountCombinations(nums, target));

    private static int CountCombinations(int[] nums, int target)
    {
        return Memoizer.Memoize<int, int>(target, WaysFor);

        int WaysFor(int remaining, Func<int, int> ways)
        {
            if (remaining == 0)
            {
                return 1;
            }

            var total = 0;
            foreach (var num in nums)
            {
                if (num <= remaining)
                {
                    total += ways(remaining - num);
                }
            }

            return total;
        }
    }
}
