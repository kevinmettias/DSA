using DSAExperimentation.LeetCode.MaximumSubarraySumAfterAtMostKSwaps;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumSubarraySumAfterAtMostKSwaps;

// Harness only. Both strategies are MaximumSubarraySumAfterAtMostKSwapsSolution's -
// this file just pins them to LeetCode's published examples, including example 3's
// swapBudget = 0 case, where no swap is available at all.
public sealed class MaximumSubarraySumAfterAtMostKSwapsTests
{
    public static TheoryData<int[], int, long> Examples =>
        new()
        {
            { [1, -1, 0, 2], 1, 3 },
            { [4, 3, 2, 4], 2, 13 },
            { [-1, -2], 0, -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSumByBruteForce_LeetCodeExamples_ReturnsBestSubarraySumAfterSwaps(
        int[] nums, int swapBudget, long expected)
    {
        var actual = MaximumSubarraySumAfterAtMostKSwapsSolution.MaxSumByBruteForce(nums, swapBudget);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSumByOrderStatisticsFenwick_LeetCodeExamples_ReturnsBestSubarraySumAfterSwaps(
        int[] nums, int swapBudget, long expected)
    {
        var actual = MaximumSubarraySumAfterAtMostKSwapsSolution.MaxSumByOrderStatisticsFenwick(nums, swapBudget);

        Assert.Equal(expected, actual);
    }
}
