using DSAExperimentation.LeetCode.SubarrayProductLessThanK;

namespace DSAExperimentation.LeetCode.Tests.SubarrayProductLessThanK;

// Harness only. Both strategies are SubarrayProductLessThanKSolution's. The first two rows are
// LeetCode's published examples. The last three each contain a subarray whose product is exactly
// the limit - [5, 6], [2, 3, 5], [5, 3, ...] at 30 - which must not count; they are the inputs on
// which a sum of logarithms counted one too many. Their counts are by hand: [5, 6] keeps [5] and
// [6]; [2, 3, 5] keeps its three singles, [2, 3] and [3, 5]; and in [5, 3, 5, 4, 10, 5, 6] the
// products under 30 are the seven singles, [5, 3], [3, 5] and [5, 4] ([5, 6] is exactly 30).
public sealed partial class SubarrayProductLessThanKSolutionTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [10, 5, 2, 6], 100, 8 },
            { [1, 2, 3], 0, 0 },
            { [5], 10, 1 },
            { [5, 6], 30, 2 },
            { [2, 3, 5], 30, 5 },
            { [5, 3, 5, 4, 10, 5, 6], 30, 10 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountSubarraysWithProductLessThanKByBruteForce_LeetCodeExamples_ReturnsValidSubarrayCount(
        int[] nums, int productLimit, int expected)
    {
        var actual = SubarrayProductLessThanKSolution.CountSubarraysWithProductLessThanKByBruteForce(nums, productLimit);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountSubarraysWithProductLessThanKBySlidingWindow_LeetCodeExamples_ReturnsValidSubarrayCount(
        int[] nums, int productLimit, int expected)
    {
        var actual = SubarrayProductLessThanKSolution.CountSubarraysWithProductLessThanKBySlidingWindow(nums, productLimit);

        Assert.Equal(expected, actual);
    }
}
