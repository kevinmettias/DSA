using DSAExperimentation.LeetCode.Solutions.SubarrayProductLessThanK;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SubarrayProductLessThanK;

// Harness only. Both strategies are SubarrayProductLessThanKSolution's. The first row is a single
// element under the limit. The last three each contain a subarray whose product is exactly
// the limit - [5, 6], [2, 3, 5], [5, 3, ...] at 30 - which must not count; they are the inputs on
// which a sum of logarithms counted one too many. Their counts are by hand: [5, 6] keeps [5] and
// [6]; [2, 3, 5] keeps its three singles, [2, 3] and [3, 5]; and in [5, 3, 5, 4, 10, 5, 6] the
// products under 30 are the seven singles, [5, 3], [3, 5] and [5, 4] ([5, 6] is exactly 30).
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class SubarrayProductLessThanKSolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            { [5], 10, 1 },
            { [5, 6], 30, 2 },
            { [2, 3, 5], 30, 5 },
            { [5, 3, 5, 4, 10, 5, 6], 30, 10 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountSubarraysWithProductLessThanKByBruteForce_Cases_ReturnsValidSubarrayCount(
        int[] nums, int productLimit, int expected)
    {
        var actual = SubarrayProductLessThanKSolution.CountSubarraysWithProductLessThanKByBruteForce(nums, productLimit);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountSubarraysWithProductLessThanKBySlidingWindow_Cases_ReturnsValidSubarrayCount(
        int[] nums, int productLimit, int expected)
    {
        var actual = SubarrayProductLessThanKSolution.CountSubarraysWithProductLessThanKBySlidingWindow(nums, productLimit);

        Assert.Equal(expected, actual);
    }
}
